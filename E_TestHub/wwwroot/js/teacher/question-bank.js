/**
 * Question Bank - Statistics Counter Animation
 * Animates numbers from 0 to target value when page loads
 */

document.addEventListener('DOMContentLoaded', function() {
    initStatCounters();
});

/**
 * Initialize counter animation for all stat cards
 */
function initStatCounters() {
    const statNumbers = document.querySelectorAll('.exam-stat-number[data-target]');
    
    // Use Intersection Observer for animation trigger when element is visible
    const observerOptions = {
        threshold: 0.5,
        rootMargin: '0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    statNumbers.forEach(counter => {
        observer.observe(counter);
    });
}

/**
 * Animate single counter from 0 to target value
 * @param {HTMLElement} element - The counter element to animate
 */
function animateCounter(element) {
    const target = parseInt(element.getAttribute('data-target'));
    const duration = 2000; // 2 seconds
    const frameDuration = 1000 / 60; // 60 FPS
    const totalFrames = Math.round(duration / frameDuration);
    let frame = 0;

    const counter = setInterval(() => {
        frame++;
        const progress = frame / totalFrames;
        
        // Easing function - ease out cubic
        const easeProgress = 1 - Math.pow(1 - progress, 3);
        const currentCount = Math.round(easeProgress * target);

        element.textContent = currentCount;

        if (frame === totalFrames) {
            clearInterval(counter);
            element.textContent = target;
        }
    }, frameDuration);
}

/**
 * Format number with commas for thousands
 * @param {number} num - Number to format
 * @returns {string} Formatted number string
 */
function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
