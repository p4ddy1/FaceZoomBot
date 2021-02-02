package de.p4ddy.facezoombot

import com.xenomachina.argparser.ArgParser
import de.p4ddy.facezoombot.core.command.Processor
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.core.transport.amqp.AmqpTransport
import de.p4ddy.facezoombot.core.transport.amqp.ConnectionSettings
import de.p4ddy.facezoombot.core.transport.serialization.CommandSerializer
import de.p4ddy.facezoombot.core.transport.serialization.JsonCommandSerializer
import de.p4ddy.facezoombot.test.TestCommand
import de.p4ddy.facezoombot.test.TestHandler
import org.koin.core.component.KoinApiExtension
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.koin.core.context.startKoin
import org.koin.dsl.module
import org.opencv.core.Core

class CmdArguments(parser: ArgParser) {
    val consume by parser.flagging(
        "-c", "--consume", help = "Consumes and handles commands from the transport"
    )

    val telegram by parser.flagging(
        "-t", "--telegram", help = "Listen to events from the Telegram API"
    )

    val verbose by parser.flagging(
        "-v", "--verbose", help = "Verbose logging"
    )
}

@KoinApiExtension
class FaceZoomBotApplication : KoinComponent {
    val transport by inject<TransportBase>()

    private val testHandler by inject<TestHandler>()

    private fun registerCommandHandler() {
        transport.subscribeHandler(TestCommand::class, testHandler)
    }

    fun startAsConsumer() {
        println("Starting as consumer...")
        System.loadLibrary(Core.NATIVE_LIBRARY_NAME)
        this.registerCommandHandler()
        transport.startConsume()
    }
}

val faceZoomBotModule = module {
    single { ConnectionSettings() }
    single { JsonCommandSerializer() as CommandSerializer }
    single { Processor() }
    single { AmqpTransport(get(), get(), get()) as TransportBase }
}

val handlerModule = module {
    single { TestHandler() }
}

@KoinApiExtension
fun main(args: Array<String>) {
    val cmdArguments = ArgParser(args).parseInto(::CmdArguments)

    if (cmdArguments.verbose) {
        System.setProperty(org.slf4j.impl.SimpleLogger.DEFAULT_LOG_LEVEL_KEY, "TRACE")
    }

    startKoin {
        printLogger()
        modules(handlerModule)
        modules(faceZoomBotModule)

        if (cmdArguments.consume) {
            FaceZoomBotApplication().startAsConsumer()
        }
    }

    println("Please start the application either as consumer or as Telegram event listener. See --help for more info")
}