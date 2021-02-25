package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.Config
import com.uchuhimo.konf.source.toml
import kotlin.properties.Delegates

class ConfigProvider(path: String = "") {
    var config = Config{
        addSpec(TelegramSpec)
        addSpec(RabbitMqSpec)
        addSpec(MongoDbSpec)
        addSpec(OpenCVSpec)
    }

    init {
        this.config = this.config.from.env()

        if(path != "") {
            this.config = this.config.from.toml.file(path)
        }
    }
}