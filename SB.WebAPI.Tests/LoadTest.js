import http from 'k6/http';

/*
Load test designed to:
- Benchmark the API
- Determine how well the API behaves during normal circumstances
 */

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        { duration: '2m', target: 100 },
        { duration: '7m', target: 150 },
        { duration: '5m', target: 0 }
    ],
    thresholds: {
        http_req_duration: ['p(85)<300ms']
    }
};

const API_BASE_URL = "http://vps.qvistgaard.me:8980/api";

export default() => {
    http.batch([
        ['GET', `${API_BASE_URL}/livechat`],
        ['GET', `${API_BASE_URL}/ticket`]
    ]);

    sleep(1);
};