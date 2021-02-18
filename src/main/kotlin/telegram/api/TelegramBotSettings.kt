package de.p4ddy.facezoombot.telegram.api

import de.p4ddy.facezoombot.config.ConfigProvider
import de.p4ddy.facezoombot.config.TelegramSpec

class TelegramBotSettings (configProvider: ConfigProvider) {
    val apiToken = configProvider.config[TelegramSpec.apiToken]
}