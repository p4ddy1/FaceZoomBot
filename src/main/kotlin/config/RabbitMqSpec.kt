package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.ConfigSpec

object RabbitMqSpec : ConfigSpec("RabbitMQ") {
    val host by optional("localhost")
    val port by optional(5672)
    val username by optional("guest")
    val password by optional("guest")
}