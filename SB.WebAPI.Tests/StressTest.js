import http from 'k6/http';

/*
Stress test designed to:
- Determine how the API will behave during extreme conditions
 */

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        { duration: '2m', target: 100 },
        { duration: '5m', target: 150 },
        { duration: '2m', target: 250 },
        { duration: '5m', target: 250 },
        { duration: '2m', target: 350 },
        { duration: '5m', target: 350 },
        { duration: '2m', target: 550 },
        { duration: '5m', target: 550 },
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