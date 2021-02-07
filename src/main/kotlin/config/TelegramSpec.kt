package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.ConfigSpec

object TelegramSpec : ConfigSpec("telegram") {
    val apiToken by required<String>()
}