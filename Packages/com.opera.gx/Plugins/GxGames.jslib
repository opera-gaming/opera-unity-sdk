mergeInto(LibraryManager.library, {
    JsTriggerPayment: function(idPtr, callbackReceiverPtr) {
        const id = UTF8ToString(idPtr)
        const callbackReceiver = UTF8ToString(callbackReceiverPtr)

        window.onPaymentCompleted = (id) => {
            SendMessage(callbackReceiver, "OnPaymentCompleted", id)
        }

        if (!window.triggerPayment) {
            console.error('Function "window.triggerPayment" is not defined.')
            window.onPaymentCompleted(id)
            return
        }

        window.triggerPayment(id)
    },
    JsGetFullVersionPaymentStatus: function(callbackReceiverPtr, apiDomainPtr) {
        Module.JsGetFullVersionPaymentStatus(callbackReceiverPtr, apiDomainPtr)
    },
    JSGetQueryParam: function(paramPtr) {
        return Module.JSGetQueryParam(paramPtr)
    },
    JSChallengeSubmitScore: function(callbackReceiverPtr, score, hashPtr, apiDomainPtr, challengeIdPtr) {
        Module.JSChallengeSubmitScore(callbackReceiverPtr, score, hashPtr, apiDomainPtr, challengeIdPtr)
    },
    JSChallengeGetScores: function(
        callbackReceiverPtr,
        apiDomainPtr, 
        endpontEndingPtr, 
        page, 
        pageSize, 
        challengeIdPtr, 
        trackIdPtr
    ) {
        Module.JSChallengeGetScores(
            callbackReceiverPtr,
            apiDomainPtr, 
            endpontEndingPtr, 
            page, 
            pageSize, 
            challengeIdPtr, 
            trackIdPtr
        )
    },
    JSChallengeGetChallenges: function(callbackReceiverPtr, apiDomainPtr, page, pageSize, trackIdPtr) {
        Module.JSChallengeGetChallenges(callbackReceiverPtr, apiDomainPtr, page, pageSize, trackIdPtr)
    },
    JSChallengeGetProfileInfo: function(callbackReceiverPtr, apiDomainPtr) {
        Module.JSChallengeGetProfileInfo(callbackReceiverPtr, apiDomainPtr)
    }
})
