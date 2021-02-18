package de.p4ddy.facezoombot.core.transport.amqp

import de.p4ddy.facezoombot.config.ConfigProvider
import de.p4ddy.facezoombot.config.RabbitMqSpec

class ConnectionSettings(configProvider: ConfigProvider) {
    val host = configProvider.config[RabbitMqSpec.host]
    val port = configProvider.config[RabbitMqSpec.port]
    val username = configProvider.config[RabbitMqSpec.username]
    val password = configProvider.config[RabbitMqSpec.password]
}