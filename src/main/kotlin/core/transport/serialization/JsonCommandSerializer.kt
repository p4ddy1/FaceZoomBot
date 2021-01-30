package de.p4ddy.facezoombot.core.transport.serialization

import com.fasterxml.jackson.module.kotlin.jacksonObjectMapper
import de.p4ddy.facezoombot.core.command.Command
import kotlin.reflect.KClass

class JsonCommandSerializer : CommandSerializer {
    private val json = jacksonObjectMapper()

    override fun <TCommand: Command> serialize(command: TCommand): ByteArray {
        return this.json.writeValueAsBytes(command)
    }

    override fun deserialize(data: ByteArray, type: KClass<out Command>): Command {
        return this.json.readValue(data, type.java)
    }
}