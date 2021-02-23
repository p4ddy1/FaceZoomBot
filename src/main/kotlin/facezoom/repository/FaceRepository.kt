package de.p4ddy.facezoombot.facezoom.repository

import de.p4ddy.facezoombot.facezoom.entity.Face

interface FaceRepository {
    suspend fun persist(picture: Face)
    suspend fun loadByPhotoId(id: String): List<Face>
    suspend fun deleteByPhotoId(id: String)
}