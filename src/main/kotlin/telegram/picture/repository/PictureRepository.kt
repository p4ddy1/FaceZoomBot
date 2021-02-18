package de.p4ddy.facezoombot.telegram.picture.repository

import de.p4ddy.facezoombot.telegram.picture.entity.Picture

interface PictureRepository {
    suspend fun persist(picture: Picture)
}