package de.p4ddy.facezoombot.telegram.api

import de.p4ddy.facezoombot.config.Config
import de.p4ddy.facezoombot.config.TelegramSpec

class TelegramBotSettings (config: Config) {
    val apiToken = config.config[TelegramSpec.apiToken]
}