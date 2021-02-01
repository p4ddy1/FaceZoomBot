package de.p4ddy.facezoombot

import de.p4ddy.facezoombot.core.command.Processor
import de.p4ddy.facezoombot.core.transport.amqp.AmqpTransport
import de.p4ddy.facezoombot.core.transport.amqp.ConnectionSettings
import de.p4ddy.facezoombot.core.transport.serialization.JsonCommandSerializer
import de.p4ddy.facezoombot.test.TestCommand
import de.p4ddy.facezoombot.test.TestHandler
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.koin.dsl.module

data class HelloMessageData(val message: String = "Hello Kotlin")

interface HelloService {
    fun hello(): String
}

class HelloServiceImpl(private val helloMessageData: HelloMessageData): HelloService {
    override fun hello() = "Hey, ${helloMessageData.message}"
}

class HelloApplication : KoinComponent {
    val helloService by inject<HelloService>()
    fun sayHello() = println(helloService.hello())
}

val helloModule = module {
    single { HelloMessageData() }
    single { HelloServiceImpl(get()) as HelloService }
}

fun main(args: Array<String>) {
    //System.setProperty(org.slf4j.impl.SimpleLogger.DEFAULT_LOG_LEVEL_KEY, "TRACE");

    /*startKoin {
        printLogger()
        modules(helloModule)

        HelloApplication().sayHello()
    }*/

    val processor = Processor()
    val serializer = JsonCommandSerializer()
    val transport = AmqpTransport(processor, serializer)

    val connectionSettings = ConnectionSettings()

    transport.connect(connectionSettings)

    val testCommand = TestCommand("foo")
    val testHandler = TestHandler()

    transport.subscribeHandler(TestCommand::class, testHandler)
    transport.send(testCommand)
    transport.startConsume()
}