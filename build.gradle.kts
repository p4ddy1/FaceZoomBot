import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.4.21"
    kotlin("plugin.serialization") version "1.4.21"
    application
}

group = "de.p4ddy"
version = "1.0-SNAPSHOT"

repositories {
    mavenCentral()
    jcenter()
}

dependencies {
    implementation("org.koin:koin-core:2.2.2")
    implementation("org.jetbrains.kotlin:kotlin-reflect:1.4.21")
    implementation("com.fasterxml.jackson.module:jackson-module-kotlin:2.12.+")
    implementation("com.rabbitmq:amqp-client:5.10.0")
    testImplementation(kotlin("test-junit"))
}

tasks.test {
    useJUnit()
}

tasks.withType<KotlinCompile>() {
    kotlinOptions.jvmTarget = "1.8"
}

application {
    mainClassName = "de.p4ddy.facezoombot.MainKt"
}