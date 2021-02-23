package de.p4ddy.facezoombot.telegram.picture.repository

import de.p4ddy.facezoombot.telegram.picture.entity.Photo

interface PhotoRepository {
    suspend fun persist(picture: Photo)
    suspend fun loadById(id: String): Photo?
    suspend fun delete(id: String)
}