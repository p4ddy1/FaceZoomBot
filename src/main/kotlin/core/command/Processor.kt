package de.p4ddy.facezoombot.core.command

import mu.KotlinLogging
import kotlin.reflect.KClass

class SubscribedHandler(private val handlerFunction: (c: Command) -> Unit) {
    fun handle(command: Command) {
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

    fun process(command: Command): Boolean {
        val handler = this.commandToHandlerMap[command::class]

        if (handler == null) {
            logger.warn { "No handler registered for command ${command::class}" }
            return false
        }

        try {
            handler.handle(command)
        } catch (e: Exception) {
            logger.warn { "${command::class}: Message Handling failed: ${e.message}" }
            return false;
        }

        return true;
    }
}