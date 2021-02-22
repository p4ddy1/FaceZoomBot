package de.p4ddy.facezoombot

import com.xenomachina.argparser.ArgParser
import de.p4ddy.facezoombot.config.ConfigProvider
import de.p4ddy.facezoombot.core.command.Processor
import de.p4ddy.facezoombot.core.database.mongodb.MongoDbClientProvider
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.core.transport.amqp.AmqpTransport
import de.p4ddy.facezoombot.core.transport.amqp.ConnectionSettings
import de.p4ddy.facezoombot.core.transport.serialization.CommandSerializer
import de.p4ddy.facezoombot.core.transport.serialization.JsonCommandSerializer
import de.p4ddy.facezoombot.facezoom.*
import de.p4ddy.facezoombot.facezoom.repository.FaceMongoDbRepository
import de.p4ddy.facezoombot.facezoom.repository.FaceRepository
import de.p4ddy.facezoombot.telegram.api.TelegramApiListener
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.api.TelegramBotSettings
import de.p4ddy.facezoombot.telegram.message.ReceiveMessageCommand
import de.p4ddy.facezoombot.telegram.message.ReceiveMessageHandler
import de.p4ddy.facezoombot.telegram.picture.ReceivePhotosCommand
import de.p4ddy.facezoombot.telegram.picture.ReceivePhotosHandler
import de.p4ddy.facezoombot.telegram.picture.repository.PhotoMongoDbRepository
import de.p4ddy.facezoombot.telegram.picture.repository.PhotoRepository
import org.koin.core.component.KoinApiExtension
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.koin.core.context.startKoin
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
    private val zoomFacesHandler by inject<ZoomFacesHandler>()
    private val sendFacesHandler by inject<SendFacesHandler>()

    private fun registerCommandHandler() {
        transport.subscribeHandler(ReceiveMessageCommand::class, receiveMessageHandler)
        transport.subscribeHandler(ReceivePhotosCommand::class, receivePhotosHandler)
        transport.subscribeHandler(ZoomFacesCommand::class, zoomFacesHandler)
        transport.subscribeHandler(SendFacesCommand::class, sendFacesHandler)
    }

    fun startAsConsumer() {
        println("Starting as consumer...")
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
    single { ConfigProvider(getProperty("configPath")) }
    single { JsonCommandSerializer() as CommandSerializer }
    single { Processor() }
    single { ConnectionSettings(get()) }
    single { AmqpTransport(get(), get(), get()) as TransportBase}
    single { TelegramBotSettings(get()) }
    single { TelegramBotApi(get(), get()) }
    single { TelegramApiListener(get()) }
    single { MongoDbClientProvider(get()) }
    single { PhotoMongoDbRepository(get()) as PhotoRepository}
    single { FaceZoomer(get()) }
    single { FaceMongoDbRepository(get()) as FaceRepository}
}

val handlerModule = module {
    single { ReceiveMessageHandler(get()) }
    single { ReceivePhotosHandler(get(), get(), get()) }
    single { ZoomFacesHandler(get(), get(), get(), get()) }
    single { SendFacesHandler(get(), get(), get()) }
}

@KoinApiExtension
fun main(args: Array<String>) {
    System.loadLibrary(Core.NATIVE_LIBRARY_NAME)

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