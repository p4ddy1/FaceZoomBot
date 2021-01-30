package de.p4ddy.facezoombot.core.transport

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.command.Processor
import kotlin.reflect.KClass

class DirectTransport(processor: Processor) : TransportBase(processor) {
    override fun <TCommand : Command> subscribeHandler(command: KClass<TCommand>, handler: Handler<TCommand>) {
        this.processor.registerHandler(command, handler)
    }

    override fun <TCommand: Command> send(command: TCommand) {
        //Directly process the command
        this.processor.process(command)
    }

    override fun startConsume() {
        TODO("Not yet implemented")
    }
}