package de.p4ddy.facezoombot.telegram.picture.repository

import de.p4ddy.facezoombot.core.database.mongodb.AbstractMongoDbRepository
import de.p4ddy.facezoombot.core.database.mongodb.MongoDbClientProvider
import de.p4ddy.facezoombot.telegram.picture.entity.Photo
import org.bson.types.ObjectId

class PhotoMongoDbRepository(clientProvider: MongoDbClientProvider): AbstractMongoDbRepository(clientProvider), PhotoRepository {
    override suspend fun persist(picture: Photo) {
        this.getCollection<Photo>().save(picture)
    }

    override suspend fun loadById(id: String): Photo? {
        return this.getCollection<Photo>().findOneById(ObjectId(id))
    }
}