package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.ConfigSpec

object TelegramSpec : ConfigSpec("Telegram") {
    val apiToken by required<String>()
}