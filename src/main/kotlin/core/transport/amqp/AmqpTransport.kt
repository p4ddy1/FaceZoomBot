package de.p4ddy.facezoombot.core.transport.amqp

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.command.Processor
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.core.transport.exception.SubscribeException
import de.p4ddy.facezoombot.core.transport.exception.TransportNotConnectedException
import kotlin.reflect.KClass
import com.rabbitmq.client.*
import de.p4ddy.facezoombot.core.transport.exception.NoCommandRegisteredException
import de.p4ddy.facezoombot.core.transport.serialization.CommandSerializer
import mu.KotlinLogging
import java.nio.charset.Charset

const val DELAY_QUEUE_PREFIX: String = "delay_"
const val DEAD_LETTER_QUEUE_PREFIX: String = "dead_"

private val logger = KotlinLogging.logger {}
class AmqpTransport (processor: Processor, private val commandSerializer: CommandSerializer) : TransportBase(processor) {
    private var channel: Channel? = null
    private var queueToCommandMap: MutableMap<String, KClass<out Command>> = mutableMapOf()
    private var declaredQueues: MutableList<String> = mutableListOf()

    fun connect(connectionSettings: ConnectionSettings) {
        val connectionFactory = ConnectionFactory()
        connectionFactory.host = connectionSettings.host
        connectionFactory.port = connectionSettings.port
        connectionFactory.username = connectionSettings.username
        connectionFactory.password = connectionSettings.password

        val connection = connectionFactory.newConnection()
        this.channel = connection.createChannel()
    }

    override fun <TCommand : Command> subscribeHandler(command: KClass<TCommand>, handler: Handler<TCommand>) {
        this.testConnection()

        val queueName = command.qualifiedName ?: throw SubscribeException("No qualified name for Command found")

        this.queueToCommandMap[queueName] = command
        this.declareQueue(queueName)
        this.processor.registerHandler(command, handler)
    }

    override fun <TCommand: Command> send(command: TCommand) {
        this.testConnection()

        val queue = this.queueToCommandMap.filterValues { it == command::class }.keys.firstOrNull()
            ?: throw NoCommandRegisteredException("Command ${command::class} not registered")

        this.publishCommand(command, queue)
    }

    override fun startConsume() {
        for (queue in this.queueToCommandMap) {
            val queueName = queue.key;

            this.channel?.basicConsume(
                queueName,
                false,
                DeliverCallback(::onMessageReceived),
                ConsumerShutdownSignalCallback(::onShutdown)
            )
        }
    }

    private fun deadLetterCommand(command: Command, queueName: String) {
        val deadLetterQueueName = "$DEAD_LETTER_QUEUE_PREFIX$queueName"
        this.declareQueue(deadLetterQueueName)
        this.publishCommand(command, deadLetterQueueName)
    }

    private fun delayCommand(command: Command, queueName: String) {
        command.retryCount++
        this.declareDelayQueue(queueName)
        this.publishCommand(command, "$DELAY_QUEUE_PREFIX$queueName")
    }

    private fun publishCommand(command: Command, queueName: String) {
        val serializedCommand = this.commandSerializer.serialize(command)
        this.channel?.basicPublish("", queueName, null, serializedCommand)
    }

    private fun declareDelayQueue(queueName: String, delay: Int = 3000) {
        val arguments = mapOf(
            "x-message-ttl" to delay,
            "x-dead-letter-exchange" to "",
            "x-dead-letter-routing-key" to queueName
        )

        this.declareQueue("$DELAY_QUEUE_PREFIX$queueName", arguments)
    }

    private fun declareQueue(queueName: String, arguments: Map<String, Any>? = null) {
        if (this.declaredQueues.contains(queueName)) {
            return
        }

        this.channel?.queueDeclare(
            queueName,
            false,
            false,
            false,
            arguments
        )
        this.declaredQueues.add(queueName)
    }

    private fun onMessageReceived(consumerTag: String, message: Delivery) {
        val queueName = message.envelope.routingKey;

        logger.debug { "Received message on $queueName: ${message.body.toString(Charset.defaultCharset())}" }

        val commandClass = this.queueToCommandMap[queueName] ?: throw NoCommandRegisteredException("Command not registered")
        val command = this.commandSerializer.deserialize(message.body, commandClass)

        val successful = this.processor.process(command)

        if (successful) {
            this.channel?.basicAck(message.envelope.deliveryTag, false)
        } else {
            this.channel?.basicAck(message.envelope.deliveryTag, false)

            if (command.retryCount >= command.maxRetryCount) {
                this.deadLetterCommand(command, queueName)
            } else {
                this.delayCommand(command, queueName)
            }
        }
    }

    private fun onShutdown(consumerTag: String , sig: ShutdownSignalException) {
        //TODO: Implement
    }

    private fun testConnection() {
        if (this.channel == null) {
            throw TransportNotConnectedException("The transport is not connected. Please call the connect() method first")
        }
    }
}