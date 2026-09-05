const target = process.env.API_TARGET || 'http://localhost:5000';
console.log('[Angular Proxy] Forwarding /api requests to:', target);

export default {
  '/api': {
    target: target,
    secure: false,
    changeOrigin: true,
    logLevel: 'debug'
  }
};
