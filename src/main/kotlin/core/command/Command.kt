package de.p4ddy.facezoombot.core.command

abstract class Command {
    var retryCount = 0;
    var maxRetryCount = 3;
}