package de.p4ddy.facezoombot.core.command

import kotlinx.coroutines.CoroutineExceptionHandler
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import mu.KotlinLogging
import kotlin.reflect.KClass

class SubscribedHandler(private val handlerFunction: suspend (c: Command) -> Unit) {
    suspend fun handle(command: Command) {
        this.handlerFunction(command)
    }
}

private val logger = KotlinLogging.logger {}
class Processor {
    private var commandToHandlerMap: MutableMap<KClass<out Command>, SubscribedHandler> = mutableMapOf()

    fun <TCommand: Command> registerHandler(command: KClass<TCommand>, handler: Handler<TCommand>) {
        @Suppress("UNCHECKED_CAST")
        this.commandToHandlerMap[command] = SubscribedHandler { cmd -> handler.handle(cmd as TCommand) }
    }

    fun process(command: Command, processingFinishedHandler: (successful: Boolean) -> Unit?) {
        val handler = this.commandToHandlerMap[command::class]

        if (handler == null) {
            logger.warn { "No handler registered for command ${command::class}" }
            processingFinishedHandler(false)
            return
        }

        val exceptionHandler = CoroutineExceptionHandler {_, exception ->
            logger.warn { "${command::class}: Message Handling failed: ${exception.message}" }
            processingFinishedHandler(false)
        }

        GlobalScope.launch(exceptionHandler) {
            handler.handle(command)
            processingFinishedHandler(true)
        }
    }
}