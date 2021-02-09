package de.p4ddy.facezoombot.core.transport.amqp

import de.p4ddy.facezoombot.config.Config
import de.p4ddy.facezoombot.config.RabbitMqSpec

class ConnectionSettings(config: Config) {
    val host = config.config[RabbitMqSpec.host]
    val port = config.config[RabbitMqSpec.port]
    val username = config.config[RabbitMqSpec.username]
    val password = config.config[RabbitMqSpec.password]
}