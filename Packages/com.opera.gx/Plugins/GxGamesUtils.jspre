//#region Utilities
const getParam = (paramName) => {
    const urlParams = new URLSearchParams(window.location.search)
    return urlParams.get(paramName)
}

// Please do not fold `map((ptr) => UTF8ToString(ptr))` to `map(UTF8ToString)`.
// Otherwise the simplified expression would be equivalent to `map((ptr, i) => UTF8ToString(ptr, i))`
// which is unwanted.
const UTF8ToStringMany = (pointers) => pointers.map((ptr) => UTF8ToString(ptr))

const doRequest = (fetchResource, fetchOptions, callbackReceiver) => {
    const sendErrorToUnity = () => {
        SendMessage(callbackReceiver, "HandleUnknownError")
    }

    const parseInUnity = (responseText) => SendMessage(callbackReceiver, "Parse", responseText)

    const handleResponse = (response) => {
        if (!response) {
            console.error("Error: Could not read the response")
            sendErrorToUnity()
            return
        }

        if (!response.ok) {
            console.error(`HTTP Error: ${response.status}`)
            response.text().then(parseInUnity)
            return
        }

        response.text().then(parseInUnity)
    }
    
    window.fetch(fetchResource, fetchOptions)
        .then(handleResponse)
        .catch((error) => {
            console.error(error)
            sendErrorToUnity()
        })
}
//#endregion

//#region Exported functions to be used in GxGames.jslib
Module.JsGetFullVersionPaymentStatus = function(callbackReceiverPtr, apiDomainPtr) {
    const [callbackReceiver, apiDomain] = UTF8ToStringMany([callbackReceiverPtr, apiDomainPtr])
    const gameId = getParam("game")

    doRequest(
        `${apiDomain}gg/games/${gameId}/full-version`,
        { credentials: "include" },
        callbackReceiver,
    )
}

Module.JSGetQueryParam = function(paramPtr) {
    const param = UTF8ToString(paramPtr)

    const result = getParam(param) ?? ""

    // Returning the string value to C#
    var bufferSize = lengthBytesUTF8(result) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(result, buffer, bufferSize);
    return buffer
}

Module.JSChallengeSubmitScore = function(callbackReceiverPtr, score, hashPtr, apiDomainPtr, challengeIdPtr) {
    const [callbackReceiver, hash, apiDomain, challengeId] = UTF8ToStringMany([callbackReceiverPtr, hashPtr, apiDomainPtr, challengeIdPtr])
    
    const gameId = getParam("game")
    const releaseTrackId = getParam("track")

    doRequest(
        `${apiDomain}gg/v2/games/${gameId}/challenges/${challengeId}/scores`,
        {
            body: JSON.stringify({
                score,
                hash,
                releaseTrackId,
            }),
            credentials: 'include',
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
        },
        callbackReceiver,
    )
}

Module.JSChallengeGetScores = function(
    callbackReceiverPtr,
    apiDomainPtr, 
    endpontEndingPtr, 
    page, 
    pageSize, 
    challengeIdPtr, 
    trackIdPtr
) {
    const [
        callbackReceiver,
        apiDomain,
        endpontEnding, 
        challengeId, 
        trackId
    ] = UTF8ToStringMany([
        callbackReceiverPtr,
        apiDomainPtr, 
        endpontEndingPtr, 
        challengeIdPtr, 
        trackIdPtr
    ])

    const gameId = getParam("game")

    doRequest(
        `${apiDomain}gg/games/${gameId}/challenges/${challengeId}/${endpontEnding}?trackId=${trackId}&page=${page}&pageSize=${pageSize}`,
        { credentials: "include" },
        callbackReceiver
    )
}

Module.JSChallengeGetChallenges = function(callbackReceiverPtr, apiDomainPtr, page, pageSize, trackIdPtr) {
    const [
        callbackReceiver,
        apiDomain,
        trackId
    ] = UTF8ToStringMany([callbackReceiverPtr, apiDomainPtr, trackIdPtr])

    const gameId = getParam("game")

    doRequest(
        `${apiDomain}gg/games/${gameId}/challenges?trackId=${trackId}&page=${page}&pageSize=${pageSize}`,
        { credentials: "include" },
        callbackReceiver,
    )
}

Module.JSChallengeGetProfileInfo = function(callbackReceiverPtr, apiDomainPtr) {
    const [callbackReceiver, apiDomain] = UTF8ToStringMany([callbackReceiverPtr, apiDomainPtr])

    doRequest(
        `${apiDomain}gg/profile`,
        { credentials: "include" },
        callbackReceiver,
    )
}
//#endregion
