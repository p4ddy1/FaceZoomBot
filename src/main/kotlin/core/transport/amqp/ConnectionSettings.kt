package de.p4ddy.facezoombot.core.transport.amqp

data class ConnectionSettings(
    val host: String = "localhost",
    val port: Int = 5672,
    val username: String = "guest",
    val password: String = "guest")