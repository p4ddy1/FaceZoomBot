package de.p4ddy.facezoombot.telegram.model

import com.github.kotlintelegrambot.entities.User as TelegramUser

data class User(
    val id: Long,
    val username: String?,
    val firstName: String?,
    val lastName: String?
) {
    companion object Factory {
        fun fromTelegramUser(user: TelegramUser): User {
            return User(
                user.id,
                user.username,
                user.firstName,
                user.lastName
            )
        }
    }
}
