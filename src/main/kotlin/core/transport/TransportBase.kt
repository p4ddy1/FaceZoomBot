package de.p4ddy.facezoombot.core.transport

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.command.Processor
import kotlin.reflect.KClass

abstract class TransportBase(protected val processor: Processor) {
    abstract fun <TCommand: Command> subscribeHandler(command: KClass<TCommand>, handler: Handler<TCommand>)
    abstract fun <TCommand: Command> send(command: TCommand)
    abstract fun startConsume()
}