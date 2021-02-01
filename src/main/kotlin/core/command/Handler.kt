package de.p4ddy.facezoombot.core.command

interface Handler<TCommand> {
    suspend fun handle(command: TCommand)
}