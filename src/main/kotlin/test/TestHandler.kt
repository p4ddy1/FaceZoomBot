package de.p4ddy.facezoombot.test

import de.p4ddy.facezoombot.core.command.Handler
import kotlinx.coroutines.delay
import java.time.LocalDateTime

class TestHandler : Handler<TestCommand> {
    override suspend fun handle(command: TestCommand) {
        val current = LocalDateTime.now()
        println("${command.testData}: $current")
        delay(5000L)
        println("${command.testData}: Done")
    }
}