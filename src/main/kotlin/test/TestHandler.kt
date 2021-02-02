package de.p4ddy.facezoombot.test

import de.p4ddy.facezoombot.core.command.Handler

class TestHandler : Handler<TestCommand> {
    override suspend fun handle(command: TestCommand) {
        println("Test!: ${command.testData}")
    }
}