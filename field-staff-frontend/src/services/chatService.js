import api from "./api"

const sendMessage = async (message, history = []) => {
    const response = await api.post("/Chat", {
        "Message": message,
        "History": history

    });

    return response.data;
} 

const chatService = {
    sendMessage,
}

export default chatService;