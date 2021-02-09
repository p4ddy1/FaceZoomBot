package de.p4ddy.facezoombot.telegram.api

class TelegramApiListener(private val telegramBotApi: TelegramBotApi) {
    fun startListening() {
        telegramBotApi.bot.startPolling()
    }
}