package de.p4ddy.facezoombot.facezoom.entity

import org.bson.codecs.pojo.annotations.BsonId
import org.litote.kmongo.Id
import org.litote.kmongo.newId
import java.util.*

class Face(
    @BsonId val key: Id<Face> = newId(),
    val photoId: String,
    val data: ByteArray,
    val createdAt: Date
)