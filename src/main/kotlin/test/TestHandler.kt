package de.p4ddy.facezoombot.test

import de.p4ddy.facezoombot.core.command.Handler

class TestHandler : Handler<TestCommand> {
    override fun handle(command: TestCommand) {
        print("Data: ${command.testData}")
    }
}