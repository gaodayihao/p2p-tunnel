import { sendWebsocketMsg } from "./request";

export const getConfig = () => {
    return sendWebsocketMsg(`vea/get`);
}
export const setConfig = (data) => {
    return sendWebsocketMsg(`vea/set`, data);
}
export const runVea = (data) => {
    return sendWebsocketMsg(`vea/run`, data);
}
export const getList = () => {
    return sendWebsocketMsg(`vea/list`);
}
export const reset = (id) => {
    return sendWebsocketMsg(`vea/reset`, id);
}
export const update = () => {
    return sendWebsocketMsg(`vea/update`);
}

export const getOnlines = (id) => {
    return sendWebsocketMsg(`vea/GetOnline`, id);
}

export const onlines = (id) => {
    return sendWebsocketMsg(`vea/onlines`, id);
}

export const test = (host, port) => {
    return sendWebsocketMsg(`vea/test`, { host, port });
}