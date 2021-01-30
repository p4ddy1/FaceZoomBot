package de.p4ddy.facezoombot.core.command

interface Handler<TCommand> {
    fun handle(command: TCommand)
}