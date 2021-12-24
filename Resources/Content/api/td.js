if (!("TD" in window)) {
	throw "Missing TD in global scope";
}

/**
 * @typedef TD
 * @type {Object}
 *
 * @property {TD_Clients} clients
 * @property {TD_Components} components
 * @property {TD_Controller} controller
 * @property {TD_Languages} languages
 * @property {TD_Metrics} metrics
 * @property {Map<string, string>|null} mustaches
 * @property {TD_Services} services
 * @property {TD_Settings} settings
 * @property {TD_UI} ui
 * @property {TD_Util} util
 * @property {TD_VO} vo
 *
 * @property {boolean} ready
 */

/**
 * @typedef TD_Clients
 * @type {Object}
 *
 * @property {function(key: string): TwitterClient|null} getClient
 * @property {function(): TwitterClient} getPreferredClient
 */

/**
 * @typedef TD_Column
 * @type {Object}
 *
 * @property {string} [_tduck_icon]
 * @property {function(id: string): ChirpBase|null} findChirp
 * @property {function(id: string): ChirpBase|null} findMostInterestingChirp
 * @property {function: string} getMediaPreviewSize
 * @property {TD_Column_Model} model
 * @property {boolean} notificationsDisabled
 * @property {function} reloadTweets
 * @property {{ columnWidth: number }} visibility
 */

/**
 * @typedef TD_Column_Model
 * @type {Object}
 *
 * @property {function: boolean} getHasNotification
 * @property {function: boolean} getHasSound
 * @property {function: string} getKey
 * @property {{ key: string, apiid: string }} privateState
 * @property {function(timestamp: number)} setClearedTimestamp
 */

/**
 * @typedef TD_Components
 * @type {Object}
 *
 * @property {Class<BaseModal>} BaseModal
 * @property {Class} ConversationDetailView
 * @property {Class<MediaGallery>} MediaGallery
 */

/**
 * @typedef TD_Controller
 * @type {Object}
 *
 * @property {TD_Controller_ColumnManager} columnManager
 * @property {TD_Controller_Init} init
 * @property {TD_Controller_Notifications} notifications
 * @property {TD_Controller_Stats} stats
 */

/**
 * @typedef TD_Controller_ColumnManager
 * @type {Object}
 *
 * @property {string[]} _columnOrder
 * @property {function(id: string): TD_Column|null} get
 * @property {function: Map<string, TD_Column>} getAll
 * @property {function: TD_Column[]} getAllOrdered
 * @property {function(id: string): TD_Column|null} getByApiid
 * @property {function(id: string, direction: "left")} move
 * @property {function(modelKey: string)} showColumn
 */

/**
 * @typedef TD_Controller_Init
 * @type {Object}
 *
 * @property {function} showLogin
 */

/**
 * @typedef TD_Controller_Notifications
 * @type {Object}
 *
 * @property {function: boolean} hasNotifications
 * @property {function: boolean} isPermissionGranted
 */

/**
 * @typedef TD_Controller_Stats
 * @type {Object}
 *
 * @property {function} quoteTweet
 */

/**
 * @typedef TD_Languages
 * @type {Object}
 *
 * @property {function: string[]} getSupportedTranslationSourceLanguages
 */

/**
 * @typedef TD_Metrics
 * @type {Object}
 *
 * @property {function} inflate
 * @property {function} inflateMetricTriple
 * @property {function} log
 * @property {function} makeKey
 * @property {function} send
 */

/**
 * @typedef TD_Services
 * @type {Object}
 *
 * @property {ChirpBase_Class} ChirpBase
 * @property {TwitterActionFollow_Class} TwitterActionFollow
 * @property {Class} TwitterActionRetweet
 * @property {Class} TwitterActionRetweetedInteraction
 * @property {Class<TwitterClient>} TwitterClient
 * @property {Class<TwitterConversation>} TwitterConversation
 * @property {Class} TwitterConversationMessageEvent
 * @property {TwitterMedia_Class} TwitterMedia
 * @property {Class<TwitterStatus>} TwitterStatus
 * @property {Class<TwitterUser>} TwitterUser
 */

/**
 * @typedef TD_Settings
 * @type {Object}
 *
 * @property {function: boolean} getComposeStayOpen
 * @property {function: boolean} getDisplaySensitiveMedia
 * @property {function: string} getFontSize
 * @property {function: string} getTheme
 * @property {function(boolean)} setComposeStayOpen
 * @property {function(string)} setFontSize
 * @property {function(string)} setTheme
 */

/**
 * @typedef TD_UI
 * @type {Object}
 *
 * @property {Object} columns
 * @property {TD_UI_Updates} updates
 */

/**
 * @typedef TD_UI_Updates
 * @type {Object}
 *
 * @property {function(column: TD_Column, chirp: ChirpBase, parentChirp: ChirpBase)} showDetailView
 */

/**
 * @typedef TD_Util
 * @type {Object}
 *
 * @property {function(a: any, b: any): number} chirpReverseColumnSort
 * @property {function} getOSName
 */

/**
 * @typedef TD_VO
 * @type {Object}
 *
 * @property {Class} Column
 */

/**
 * @typedef BaseModal
 * @typedef {Object}
 */

/**
 * @typedef ChirpBase
 * @type {Object}
 *
 * @property {TwitterUser_Account} account
 * @property {string} chirpType
 * @property {function: string} getChirpType
 * @property {function: string} getChirpURL
 * @property {function: ChirpBase|null} getMainTweet
 * @property {function: TwitterUser} getMainUser
 * @property {function: TwitterMedia[]} getMedia
 * @property {function: ChirpBase|null} getRelatedTweet
 * @property {function: TwitterUser[]} getReplyUsers
 * @property {function: boolean} hasImage
 * @property {function: boolean} hasMedia
 * @property {string} id
 * @property {function: boolean} isAboutYou
 * @property {TwitterConversationMessageEvent[]} [messages]
 * @property {boolean} possiblySensitive
 * @property {ChirpBase|null} quotedTweet
 * @property {function(ChirpRenderSettings)} render
 * @property {TwitterUser} user
 */

/**
 * @typedef ChirpBase_Class
 * @type {Class<ChirpBase>}
 *
 * @property {string} TWEET
 */

/**
 * @typedef ChirpRenderSettings
 * @type {Object}
 *
 * @property {boolean} withFooter
 * @property {boolean} withTweetActions
 * @property {boolean} isInConvo
 * @property {boolean} isFavorite
 * @property {boolean} isRetweeted
 * @property {boolean} isPossiblySensitive
 * @property {string} mediaPreviewSize
 */

/**
 * @typedef MediaGallery
 * @type {Object}
 *
 * @property {ChirpBase} chirp
 * @property {string} clickedMediaEntityId
 */

/**
 * @typedef TwitterActionFollow
 * @type {Object}
 *
 * @property {TwitterUser} following
 */

/**
 * @typedef TwitterActionFollow_Class
 * @type {Class<TwitterActionFollow>}
 */

/**
 * @typedef TwitterCardJSON
 * @type {Object}
 *
 * @property {{ [card_url]: { [string_value]: string } }} binding_values
 */

/**
 * @typedef TwitterClient
 * @type {Object}
 *
 * @property {function(chirp: ChirpBase)} callback
 * @property {string} chirpId
 * @property {TwitterConversations} conversations
 * @property {function(ids: string[], onSuccess: function(users: TwitterUser[]), onError: function)} getUsersByIds
 */

/**
 * @typedef TwitterConversation
 * @type {Object}
 *
 * @property {function} markAsRead
 * @property {Array} messages
 */

/**
 * @typedef TwitterConversationMessageEvent
 * @extends ChirpBase
 * @type {Object}
 */

/**
 * @typedef TwitterConversations
 * @type {Object}
 *
 * @property {function(id: string): TwitterConversation|null} getConversation
 */

/**
 * @typedef TwitterEntity_URL
 * @type {Object}
 *
 * @property {string} url
 * @property {string} expanded_url
 * @property {string} display_url
 * @property {string} indices
 */

/**
 * @typedef TwitterMedia
 * @type {Object}
 *
 * @property {function: { url: string }} chooseVideoVariant
 * @property {{ media_url_https: string }} entity
 * @property {boolean} isAnimatedGif
 * @property {boolean} isVideo
 * @property {function: string} large
 * @property {string} mediaId
 * @property {string} service
 * @property {function: string} small
 */

/**
 * @typedef TwitterMedia_Class
 * @type {Class<TwitterMedia>}
 *
 * @property {RegExp} YOUTUBE_TINY_RE
 * @property {RegExp} YOUTUBE_LONG_RE
 * @property {RegExp} YOUTUBE_RE
 * @property {{ youtube: RegExp }} SERVICES
 */

/**
 * @typedef TwitterStatus
 * @type {Object}
 *
 * @property {TwitterCardJSON|null} card
 * @property {{ urls: TwitterEntity_URL[] }} entities
 */

/**
 * @typedef TwitterUser
 * @type {Object}
 *
 * @property {string} emojifiedName
 * @property {TwitterUserEntities} entities
 * @property {function: string} getProfileURL
 * @property {string} id
 * @property {string} name
 * @property {string} profileImageURL
 * @property {string} screenName
 * @property {string} url
 */

/**
 * @typedef TwitterUserJSON
 * @type {Object}
 *
 * @property {string} id
 * @property {string} id_str
 * @property {string} name
 * @property {string} profile_image_url
 * @property {string} profile_image_url_https
 */

/**
 * @typedef TwitterUserEntities
 * @type {Object}
 *
 * @property {{ urls: TwitterEntity_URL[] }} url
 */

/**
 * @typedef TwitterUser_Account
 * @type {Object}
 *
 * @property {function: string} getKey
 */

/** @type {TD} */
export const TD = window.TD;
