package de.p4ddy.facezoombot.core.database.mongodb

import de.p4ddy.facezoombot.config.ConfigProvider
import de.p4ddy.facezoombot.config.MongoDbSpec
import org.litote.kmongo.coroutine.CoroutineDatabase
import org.litote.kmongo.coroutine.coroutine
import org.litote.kmongo.reactivestreams.KMongo

class MongoDbClientProvider(private val configProvider: ConfigProvider) {
    val client = KMongo.createClient(configProvider.config[MongoDbSpec.connectionString]).coroutine

    fun getDatabase(): CoroutineDatabase {
        return this.client.getDatabase(this.configProvider.config[MongoDbSpec.database])
    }
}