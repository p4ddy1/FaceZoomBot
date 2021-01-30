package de.p4ddy.facezoombot.core.transport.serialization

import de.p4ddy.facezoombot.core.command.Command
import kotlin.reflect.KClass

interface CommandSerializer {
    fun <TCommand: Command> serialize (command: TCommand): ByteArray
    fun deserialize(data: ByteArray, type: KClass<out Command>): Command
}