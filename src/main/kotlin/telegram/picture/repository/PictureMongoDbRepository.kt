package de.p4ddy.facezoombot.telegram.picture.repository

import de.p4ddy.facezoombot.core.database.mongodb.MongoDbClientProvider
import de.p4ddy.facezoombot.telegram.picture.entity.Picture

class PictureMongoDbRepository(private val clientProvider: MongoDbClientProvider) : PictureRepository {
    override suspend fun persist(picture: Picture) {
        val collection = this.clientProvider.getDatabase().getCollection<Picture>()
        collection.save(picture)
    }
}