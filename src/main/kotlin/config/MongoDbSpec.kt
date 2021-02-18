package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.ConfigSpec

object MongoDbSpec : ConfigSpec("MongoDB") {
    val connectionString by required<String>()
    val database by required<String>()
}