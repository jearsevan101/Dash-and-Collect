function InitModule(ctx, logger, nk, initializer) {
    logger.info("submit_run.js loaded");
    initializer.registerRpc("SubmitRun", submitRunRpc);

    try {
        nk.leaderboardCreate(
            "completion_leaderboard", // ID
            false,                    // authoritative
            "asc",                    // sort order
            "best",                   // operator
            "",                       // reset schedule (no reset)
            {}                        // metadata
        );
        logger.info("Leaderboard 'completion_leaderboard' created or already exists.");
    } catch (err) {
        logger.error("Failed to create leaderboard: %s", err);
    }
}

function submitRunRpc(ctx, logger, nk, payload) {
    try {
        let data = JSON.parse(payload);

        if (data.completionTimeMs < 15000) {
            return JSON.stringify({ success: false, error: "Invalid run: too fast!" });
        }
        if (data.hitsTaken < 0) {
            return JSON.stringify({ success: false, error: "Invalid run: hits < 0!" });
        }

        nk.leaderboardRecordWrite(
            "completion_leaderboard",
            ctx.userId,
            ctx.username || "Player",
            data.completionTimeMs,
            0, // subscore
            {
                hitsTaken: data.hitsTaken,
                version: data.clientVersion || "1.0.0"
            }
        );

        return JSON.stringify({ success: true });
    } catch (error) {
        logger.error("Error in SubmitRun RPC: %s", error.message);
        return JSON.stringify({ success: false, error: error.message });
    }
}

// global.InitModule = function (ctx, logger, nk, initializer) {
//     initializer.registerRpc("SubmitRun", submitRunRpc);

//     try {
//         nk.leaderboardCreate(
//             "completion_leaderboard",
//             false,
//             "asc",
//             "best",
//             "",
//             {}
//         );
//         logger.info("Leaderboard 'completion_leaderboard' created or already exists.");
//     } catch (err) {
//         logger.error("Failed to create leaderboard: %s", err);
//     }
// }



// function InitModule(ctx, logger, nk, initializer) {
//     logger.info("submit_run.js loaded");
//     initializer.registerRpc("SubmitRun", submitRunRpc);

//     try {
//         nk.leaderboardCreate(
//             "completion_leaderboard", // ID
//             false,                    // authoritative
//             "asc",                    // sort order
//             "best",                   // operator
//             "",                       // reset schedule (no reset)
//             {}                        // metadata
//         );
//         logger.info("Leaderboard 'completion_leaderboard' created or already exists.");
//     } catch (err) {
//         logger.error("Failed to create leaderboard: %s", err);
//     }
// }

// function submitRunRpc(ctx, logger, nk, payload) {
//     try {
//         let data = JSON.parse(payload);

//         if (data.completionTimeMs < 15000) {
//             return JSON.stringify({ success: false, error: "Invalid run: too fast!" });
//         }
//         if (data.hitsTaken < 0) {
//             return JSON.stringify({ success: false, error: "Invalid run: hits < 0!" });
//         }

//         nk.leaderboardRecordWrite(
//             "completion_leaderboard",
//             ctx.userId,
//             ctx.username || "Player",
//             data.completionTimeMs,
//             0, // subscore
//             {
//                 hitsTaken: data.hitsTaken,
//                 version: data.clientVersion || "1.0.0"
//             }
//         );

//         return JSON.stringify({ success: true });
//     } catch (error) {
//         logger.error("Error in SubmitRun RPC: %s", error.message);
//         return JSON.stringify({ success: false, error: error.message });
//     }
// }
