package de.p4ddy.facezoombot.facezoom.repository

import de.p4ddy.facezoombot.core.database.mongodb.AbstractMongoDbRepository
import de.p4ddy.facezoombot.core.database.mongodb.MongoDbClientProvider
import de.p4ddy.facezoombot.facezoom.entity.Face
import org.litote.kmongo.eq

class FaceMongoDbRepository(clientProvider: MongoDbClientProvider): AbstractMongoDbRepository(clientProvider), FaceRepository {
    override suspend fun persist(picture: Face) {
        this.getCollection<Face>().save(picture)
    }

    override suspend fun loadByPhotoId(id: String): List<Face> {
        return this.getCollection<Face>().find(Face::photoId eq id).toList()
    }

}