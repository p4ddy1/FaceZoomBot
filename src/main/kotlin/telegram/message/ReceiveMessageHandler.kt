package de.p4ddy.facezoombot.telegram.message

import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.database.mongodb.MongoDbClientProvider

data class Message(val text: String)

class ReceiveMessageHandler(private val mongo: MongoDbClientProvider) : Handler<ReceiveMessageCommand> {
    override suspend fun handle(command: ReceiveMessageCommand) {
        val db = mongo.client.getDatabase("FaceZoomBot")
        val col = db.getCollection<Message>()

        col.insertOne(Message(command.message))
        println(command.message)
    }
}