import http from 'k6/http';

/*
Spike test designed to:
- Determine how the API will behave during sudden heavy loads of traffic
 */

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        { duration: '10s', target: 100 },
        { duration: '1m', target: 150 },
        { duration: '15s', target: 1000 },
        { duration: '2m', target: 1200 },
        { duration: '1m', target: 350 },
        { duration: '8m', target: 0 },
    ]
};

const API_BASE_URL = "http://vps.qvistgaard.me:8980/api";

export default() => {
    http.batch([
        ['GET', `${API_BASE_URL}/livechat`],
        ['GET', `${API_BASE_URL}/ticket`]
    ]);

    sleep(1);
};