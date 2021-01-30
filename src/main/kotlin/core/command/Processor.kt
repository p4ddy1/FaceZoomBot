package de.p4ddy.facezoombot.core.command

import kotlin.reflect.KClass

class SubscribedHandler(private val handlerFunction: (c: Command) -> Unit) {
    fun handle(command: Command) {
        this.handlerFunction(command)
    }
}

class Processor {
    private var commandToHandlerMap: MutableMap<KClass<out Command>, SubscribedHandler> = mutableMapOf()

    fun <TCommand: Command> registerHandler(command: KClass<TCommand>, handler: Handler<TCommand>) {
        @Suppress("UNCHECKED_CAST")
        this.commandToHandlerMap[command] = SubscribedHandler { cmd -> handler.handle(cmd as TCommand) }
    }

    fun process(command: Command) {
        val handler = this.commandToHandlerMap[command::class]

        //TODO: Handling for unmapped commands
        handler?.handle(command)
    }
}