package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.Config
import com.uchuhimo.konf.source.toml

class Config(path: String = "config.toml") {
    val config: Config = Config {
        addSpec(TelegramSpec)
        addSpec(RabbitMqSpec)
    }
        .from.toml.file(path)
        .from.env()
}