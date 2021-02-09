package de.p4ddy.facezoombot

import com.xenomachina.argparser.ArgParser
import de.p4ddy.facezoombot.config.Config
import de.p4ddy.facezoombot.config.RabbitMqSpec
import de.p4ddy.facezoombot.core.command.Processor
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.core.transport.amqp.AmqpTransport
import de.p4ddy.facezoombot.core.transport.amqp.ConnectionSettings
import de.p4ddy.facezoombot.core.transport.serialization.CommandSerializer
import de.p4ddy.facezoombot.core.transport.serialization.JsonCommandSerializer
import de.p4ddy.facezoombot.telegram.api.TelegramApiListener
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.api.TelegramBotSettings
import de.p4ddy.facezoombot.telegram.message.ReceiveMessageCommand
import de.p4ddy.facezoombot.telegram.message.ReceiveMessageHandler
import de.p4ddy.facezoombot.telegram.picture.ReceivePhotosCommand
import de.p4ddy.facezoombot.telegram.picture.ReceivePhotosHandler
import org.koin.core.component.KoinApiExtension
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.koin.core.context.startKoin
import org.koin.core.parameter.parametersOf
import org.koin.dsl.module
import org.opencv.core.Core

class CmdArguments(parser: ArgParser) {
    val config by parser.storing(
        "-c", "--config", help = "Config file path"
    )

    val consume by parser.flagging(
        "-C", "--consume", help = "Consumes and handles commands from the transport"
    )

    val telegram by parser.flagging(
        "-T", "--telegram", help = "Listen to events from the Telegram API"
    )

    val verbose by parser.flagging(
        "-v", "--verbose", help = "Verbose logging"
    )
}

@KoinApiExtension
class FaceZoomBotApplication() : KoinComponent {
    val transport by inject<TransportBase>()

    val telegramApiListener by inject<TelegramApiListener>()

    private val receiveMessageHandler by inject<ReceiveMessageHandler>()
    private val receivePhotosHandler by inject<ReceivePhotosHandler>()

    private fun registerCommandHandler() {
        transport.subscribeHandler(ReceiveMessageCommand::class, receiveMessageHandler)
        transport.subscribeHandler(ReceivePhotosCommand::class, receivePhotosHandler)
    }

    fun startAsConsumer() {
        println("Starting as consumer...")
        System.loadLibrary(Core.NATIVE_LIBRARY_NAME)
        this.registerCommandHandler()
        transport.startConsume()
    }

    fun startAsTelegramHandler() {
        println("Starting as telegram handler...")
        this.registerCommandHandler()
        telegramApiListener.startListening()
    }
}

val faceZoomBotModule = module {
    single { Config(getProperty("configPath")) }
    single { JsonCommandSerializer() as CommandSerializer }
    single { Processor() }
    single { ConnectionSettings(get()) }
    single { AmqpTransport(get(), get(), get()) as TransportBase}
    single { TelegramBotSettings(get()) }
    single { TelegramBotApi(get(), get()) }
    single { TelegramApiListener(get()) }
}

val handlerModule = module {
    single { ReceiveMessageHandler() }
    single { ReceivePhotosHandler(get()) }
}

@KoinApiExtension
fun main(args: Array<String>) {
    val cmdArguments = ArgParser(args).parseInto(::CmdArguments)

    if (cmdArguments.verbose) {
        System.setProperty(org.slf4j.impl.SimpleLogger.DEFAULT_LOG_LEVEL_KEY, "TRACE")
    }

    startKoin {
        printLogger()
        properties(mapOf("configPath" to cmdArguments.config))
        modules(handlerModule)
        modules(faceZoomBotModule)

        if (cmdArguments.consume) {
            FaceZoomBotApplication().startAsConsumer()
        } else if (cmdArguments.telegram) {
            FaceZoomBotApplication().startAsTelegramHandler()
        } else {
            println("Please start the application either as consumer or as Telegram event listener. See --help for more info")
        }
    }
}