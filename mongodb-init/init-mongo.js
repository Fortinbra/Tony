// MongoDB initialization script
// This script runs when MongoDB container starts for the first time

// Create application user for Tony Bot
db = db.getSiblingDB('TonyBot');

// Create indexes for better performance
db.users.createIndex({ "discordId": 1 }, { unique: true });
db.users.createIndex({ "username": 1 });
db.logs.createIndex({ "timestamp": 1 });
db.logs.createIndex({ "level": 1 });

print("MongoDB initialized for Tony Bot");
