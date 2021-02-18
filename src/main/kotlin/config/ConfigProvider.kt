package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.Config
import com.uchuhimo.konf.source.toml

class ConfigProvider(path: String = "config.toml") {
    val config: Config = Config {
        addSpec(TelegramSpec)
        addSpec(RabbitMqSpec)
        addSpec(MongoDbSpec)
    }
        .from.toml.file(path)
        .from.env()
}