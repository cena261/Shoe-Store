class Logger {
  constructor() {
    this.isDevelopment = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1';
  }

  error(message, context = {}) {
    if (this.isDevelopment) {
      console.error(message, context);
    }

    this.logToServer('error', message, context);
  }

  warn(message, context = {}) {
    if (this.isDevelopment) {
      console.warn(message, context);
    }
  }

  info(message, context = {}) {
    if (this.isDevelopment) {
      console.info(message, context);
    }
  }

  logToServer(level, message, context) {
    if (!this.isDevelopment) {
      return;
    }

    try {
      const logData = {
        level: level,
        message: message,
        context: context,
        url: window.location.href,
        userAgent: navigator.userAgent,
        timestamp: new Date().toISOString()
      };

      navigator.sendBeacon('/api/Log/ClientError', JSON.stringify(logData));
    } catch (e) {
    }
  }
}

const logger = new Logger();
