package de.p4ddy.facezoombot.core.database.mongodb

import org.litote.kmongo.coroutine.CoroutineCollection

abstract class AbstractMongoDbRepository(protected val clientProvider: MongoDbClientProvider) {
    protected inline fun<reified TEntity : Any> getCollection(): CoroutineCollection<TEntity> {
        return this.clientProvider.getDatabase().getCollection()
    }
}