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

class AmqpTransport (processor: Processor, private val commandSerializer: CommandSerializer) : TransportBase(processor) {
    private var channel: Channel? = null
    private var queueToCommandMap: MutableMap<String, KClass<out Command>> = mutableMapOf()

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
        this.channel?.queueDeclare(queueName, false, false, false, null)
        this.processor.registerHandler(command, handler)
    }

    override fun <TCommand: Command> send(command: TCommand) {
        this.testConnection()

        val queue = this.queueToCommandMap.filterValues { it == command::class }.keys.firstOrNull()
            ?: throw NoCommandRegisteredException("Command ${command::class} not registered")

        val serializedCommand = this.commandSerializer.serialize(command)

        this.channel?.basicPublish("", queue, null, serializedCommand)
    }

    override fun startConsume() {
        for (queue in this.queueToCommandMap) {
            val queueName = queue.key;

            this.channel?.basicConsume(
                queueName,
                true,
                DeliverCallback(::onMessageReceived),
                ConsumerShutdownSignalCallback(::onShutdown)
            )
        }
    }

    private fun onMessageReceived(consumerTag: String, message: Delivery) {
        val commandClass = this.queueToCommandMap[message.envelope.routingKey] ?: throw NoCommandRegisteredException("Command not registered")
        val command = this.commandSerializer.deserialize(message.body, commandClass)

        this.processor.process(command)
    }

    private fun onShutdown(consumerTag: String , sig: ShutdownSignalException) {

    }

    private fun testConnection() {
        if (this.channel == null) {
            throw TransportNotConnectedException("The transport is not connected. Please call the connect() method first")
        }
    }
}