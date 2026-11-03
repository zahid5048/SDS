/* Word-style formatting for a single description field. */
(function () {
    const FONTS = [
        'Times New Roman', 'Arial', 'Calibri', 'Cambria', 'Georgia',
        'Verdana', 'Tahoma', 'Trebuchet MS', 'Courier New', 'Garamond'
    ];
    const SIZES = [8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72];
    const ALLOWED = new Set(['B', 'STRONG', 'I', 'EM', 'U', 'S', 'STRIKE', 'SUB', 'SUP', 'SPAN', 'P', 'DIV', 'BR', 'UL', 'OL', 'LI', 'FONT', 'H1', 'H2', 'H3', 'H4', 'H5', 'H6', 'IMG', 'TABLE', 'THEAD', 'TBODY', 'TFOOT', 'TR', 'TH', 'TD', 'COLGROUP', 'COL']);
    const STYLE_OK = /^(color|background-color|background|font-size|font-family|font-weight|font-style|text-decoration(?:-line)?|text-align|line-height|margin-left|width|height|vertical-align|table-layout|max-width|display|align-items|gap|flex-wrap|border(?:-(?:top|right|bottom|left|collapse|color|width|style))?|padding(?:-(?:top|right|bottom|left))?)$/i;

    function isSafeImageSrc(src) {
        return /^data:image\/(png|jpeg|jpg|gif|webp);base64,[a-z0-9+/=\s]+$/i.test(src)
            || /^(\.\.\/|\/)?uploads\/sds\/[a-z0-9_./-]+$/i.test(src);
    }

    function escapeHtml(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    const DROP_TAGS = new Set(['STYLE', 'SCRIPT', 'META', 'LINK', 'TITLE', 'HEAD', 'XML', 'NOSCRIPT', 'IFRAME', 'OBJECT']);
    const AS_DIV = new Set(['SECTION', 'ARTICLE', 'HEADER', 'FOOTER', 'MAIN', 'ASIDE', 'NAV', 'HGROUP', 'FIGURE', 'FIGCAPTION', 'ADDRESS', 'BLOCKQUOTE']);

    const HIGHLIGHT_COLORS = {
        yellow: '#ffff00', green: '#00ff00', cyan: '#00ffff', magenta: '#ff00ff',
        blue: '#0000ff', red: '#ff0000', darkblue: '#00008b', darkcyan: '#008080',
        teal: '#008080', darkgreen: '#008000', darkmagenta: '#800080', violet: '#800080',
        darkred: '#800000', darkyellow: '#808000', olive: '#808000', darkgray: '#808080',
        gray: '#808080', grey: '#808080', lightgray: '#c0c0c0', lightgrey: '#c0c0c0',
        black: '#000000', white: '#ffffff', pink: '#ff00ff', turquoise: '#00ffff'
    };

    function cssColor(value) {
        const val = String(value || '').replace(/\s*!important\s*/ig, '').trim();
        if (!val || /^(auto|transparent|none)$/i.test(val)) return '';
        if (/^#[0-9a-f]{3,8}$/i.test(val) || /^(rgb|hsl)a?\(/i.test(val)) return val;
        const named = val.toLowerCase().replace(/\s+/g, '');
        return HIGHLIGHT_COLORS[named] || (/^[a-z]+$/i.test(named) ? named : '');
    }

    function filterStyle(style) {
        const out = [];
        String(style || '').split(';').forEach(part => {
            const idx = part.indexOf(':');
            if (idx < 0) return;
            const prop = part.slice(0, idx).trim().toLowerCase();
            let val = part.slice(idx + 1).trim().replace(/windowtext/gi, '#222').replace(/\s*!important\s*/ig, '');
            if (prop === 'mso-highlight') {
                const color = cssColor(val);
                if (color) out.push('background-color: ' + color);
                return;
            }
            if (!STYLE_OK.test(prop)) return;
            if (/url\(|expression\(|javascript:|mso-|panose/i.test(val)) return;
            if (prop === 'background') {
                if (!/^(#[0-9a-f]{3,8}|rgb|hsl|[a-z]+)/i.test(val)) return;
                out.push('background-color: ' + val);
                return;
            }
            if (prop === 'font-family') {
                const rawFamily = val;
                if (/bold/i.test(rawFamily)) out.push('font-weight: 700');
                if (/italic|oblique/i.test(rawFamily)) out.push('font-style: italic');
                val = val.split(',')[0].replace(/["']/g, '').replace(/\\/g, '').trim();
                val = val.replace(/(?:[-_\s]?(?:bolditalic|boldmt|italicmt|psmt|bold|italic|oblique|regular|mt))+$/i, '').trim();
                if (!val) return;
            }
            if (prop === 'font-size' && !/^[\d.]+(pt|px|em|rem|%)$/i.test(val)) return;
            if (prop === 'display' && val !== 'flex' && val !== 'block') return;
            if (prop === 'align-items' && !/^(flex-start|center|flex-end)$/.test(val)) return;
            if (prop === 'gap' && !/^[\d.]+px$/.test(val)) return;
            if (prop === 'flex-wrap' && val !== 'wrap') return;
            out.push(prop + ': ' + val);
        });
        return out.join('; ');
    }

    function stripOfficeJunk(text) {
        if (!text || !/font-face|Font Definitions|Style Definitions|mso-|panose-1|@page/i.test(text))
            return text;
        let cleaned = text.replace(/<!--[\s\S]*?-->/g, ' ');
        cleaned = cleaned.replace(/\/\*[\s\S]*?\*\//g, ' ');
        cleaned = cleaned.replace(/@[a-z-]+\s*\{[\s\S]*?\}/gi, ' ');
        cleaned = cleaned.replace(/[^{}]+\{[^}]*\}/g, block =>
            (/mso-|panose|font-family|style-name/i.test(block) ? ' ' : block));
        if (/mso-|panose|font-face|style definitions/i.test(cleaned)) {
            const last = cleaned.lastIndexOf('}');
            if (last >= 0) cleaned = cleaned.slice(last + 1);
        }
        return cleaned.replace(/[ \t]+\n/g, '\n').replace(/\n{3,}/g, '\n\n');
    }

    function selectorOk(sel) {
        return /^(?:[a-z][a-z0-9]*)?(?:\.[a-z_][\w-]*|#[a-z_][\w-]*)*$/i.test(sel) && /[a-z0-9]/i.test(sel);
    }

    function inlineClipboardCss(html) {
        const blocks = [];
        const without = String(html || '').replace(/<style[^>]*>([\s\S]*?)<\/style>/gi, (_, css) => {
            blocks.push(css);
            return '';
        });
        if (!blocks.length) return without;
        const css = blocks.join('\n').replace(/<!--|-->/g, '').replace(/\/\*[\s\S]*?\*\//g, '');
        const body = css.replace(/@[a-z-]+[^{]*\{[\s\S]*?\}/gi, '');
        const rules = [];
        const re = /([^{}]+)\{([^{}]+)\}/g;
        let match;
        while ((match = re.exec(body))) {
            const style = filterStyle(match[2]);
            if (!style) continue;
            match[1].split(',').forEach(part => {
                const sel = part.replace(/::?[a-z-]+(\([^)]*\))?/gi, '').trim();
                if (!selectorOk(sel)) return;
                rules.push({ sel, style });
            });
        }
        if (!rules.length) return without;
        const root = document.createElement('div');
        root.innerHTML = without;
        rules.forEach(rule => {
            let nodes = [];
            try { nodes = [...root.querySelectorAll(rule.sel)]; } catch { nodes = []; }
            nodes.forEach(node => {
                const existing = node.getAttribute('style') || '';
                node.setAttribute('style', rule.style + (existing ? '; ' + existing : ''));
            });
        });
        return root.innerHTML;
    }

    function cleanWordHtml(html) {
        let source = String(html || '');
        const startMatch = source.match(/<!--\s*StartFragment\s*-->/i);
        const endMatch = source.match(/<!--\s*EndFragment\s*-->/i);
        if (startMatch && endMatch && endMatch.index > startMatch.index) {
            source = source.slice(startMatch.index + startMatch[0].length, endMatch.index);
        }
        return source
            .replace(/<!--\[if[\s\S]*?<!\[endif\]-->/gi, '')
            .replace(/<style[\s\S]*?<\/style>/gi, '')
            .replace(/<script[\s\S]*?<\/script>/gi, '')
            .replace(/<xml[\s\S]*?<\/xml>/gi, '')
            .replace(/<meta\b[^>]*>/gi, '')
            .replace(/<link\b[^>]*>/gi, '')
            .replace(/<\/?(?:html|head|body)\b[^>]*>/gi, '');
    }

    function sanitizeNode(parent) {
        const children = [...parent.childNodes];
        for (const child of children) {
            if (!child.parentNode) continue;
            if (child.nodeType === Node.COMMENT_NODE) {
                child.remove();
                continue;
            }
            if (child.nodeType === Node.TEXT_NODE) {
                const cleaned = stripOfficeJunk(child.textContent || '');
                if (!cleaned.trim()) child.remove();
                else child.textContent = cleaned;
                continue;
            }
            if (child.nodeType !== Node.ELEMENT_NODE) continue;
            const tag = child.tagName;
            if (DROP_TAGS.has(tag)) {
                child.remove();
                continue;
            }
            if (AS_DIV.has(tag)) {
                const block = document.createElement('div');
                while (child.firstChild) block.appendChild(child.firstChild);
                child.replaceWith(block);
                sanitizeNode(parent);
                return;
            }
            if (tag === 'MARK') {
                const span = document.createElement('span');
                const kept = filterStyle(child.getAttribute('style') || '');
                span.setAttribute('style', (kept ? kept + '; ' : '') + 'background-color: #ffff00');
                while (child.firstChild) span.appendChild(child.firstChild);
                child.replaceWith(span);
                sanitizeNode(parent);
                return;
            }
            if (!ALLOWED.has(tag)) {
                child.replaceWith(...child.childNodes);
                sanitizeNode(parent);
                return;
            }
            [...child.attributes].forEach(attr => {
                const name = attr.name.toLowerCase();
                if (name === 'style') {
                    const style = filterStyle(attr.value);
                    if (style) child.setAttribute('style', style);
                    else child.removeAttribute('style');
                    return;
                }
                if (tag === 'IMG' && name === 'src') {
                    if (!isSafeImageSrc(attr.value)) child.removeAttribute('src');
                    return;
                }
                if (tag === 'IMG' && name === 'alt') return;
                if (tag === 'FONT' && (name === 'color' || name === 'face' || name === 'size')) return;
                if ((tag === 'TD' || tag === 'TH' || tag === 'TR' || tag === 'TABLE') && name === 'bgcolor') {
                    const color = cssColor(attr.value);
                    if (color && !/background-color/i.test(child.getAttribute('style') || '')) {
                        const style = child.getAttribute('style') || '';
                        child.setAttribute('style', (style ? style.replace(/;\s*$/, '') + '; ' : '') + 'background-color: ' + color);
                    }
                    child.removeAttribute(name);
                    return;
                }
                if ((tag === 'TD' || tag === 'TH') && (name === 'colspan' || name === 'rowspan') && /^\d{1,3}$/.test(attr.value)) return;
                if (tag === 'COL' && name === 'span' && /^\d{1,3}$/.test(attr.value)) return;
                child.removeAttribute(attr.name);
            });
            sanitizeNode(child);
            if (tag === 'IMG' && child.parentNode && !child.getAttribute('src')) child.remove();
            if (tag === 'DIV' && child.parentNode) {
                const style = child.getAttribute('style') || '';
                if (/display\s*:\s*flex/i.test(style) && !child.querySelector('table')) {
                    const cleaned = style
                        .replace(/display\s*:\s*flex\s*;?/ig, '')
                        .replace(/align-items\s*:[^;]+;?/ig, '')
                        .replace(/gap\s*:[^;]+;?/ig, '')
                        .replace(/flex-wrap\s*:[^;]+;?/ig, '')
                        .replace(/;\s*;/g, ';')
                        .trim();
                    if (cleaned && cleaned !== ';') child.setAttribute('style', cleaned);
                    else child.removeAttribute('style');
                }
            }
            if ((tag === 'SPAN' || tag === 'FONT') && child.parentNode && !child.getAttribute('style') && !child.attributes.length) {
                child.replaceWith(...child.childNodes);
            }
        }
    }

    function splitGluedHeadings(root) {
        const nodes = [];
        const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
        while (walker.nextNode()) nodes.push(walker.currentNode);
        nodes.forEach(node => {
            if (node.parentElement && node.parentElement.closest('table')) return;
            const text = node.textContent || '';
            if (!/(?<=[^\d\s])(?=\d+\.\d+\s+[A-Za-z])/.test(text)) return;
            const parts = text.split(/(?<=[^\d\s])(?=\d+\.\d+\s+[A-Za-z])/);
            if (parts.length < 2) return;
            const frag = document.createDocumentFragment();
            parts.forEach((part, index) => {
                if (index > 0) frag.appendChild(document.createElement('br'));
                frag.appendChild(document.createTextNode(part));
            });
            node.parentNode.insertBefore(frag, node);
            node.remove();
        });
    }

    function promoteBreaks(root) {
        const targets = [...root.querySelectorAll('p, div, h1, h2, h3, h4, h5, h6')].filter(block =>
            !block.closest('table')
            && !block.classList.contains('word-table-frame')
            && !block.classList.contains('word-table-row')
            && !block.querySelector('table')
            && [...block.childNodes].some(node => node.nodeType === 1 && node.tagName === 'BR'));
        if ([...root.childNodes].some(node => node.nodeType === 1 && node.tagName === 'BR')) targets.unshift(root);

        targets.forEach(block => {
            if (!block.parentNode) return;
            const nodes = [...block.childNodes];
            let trailing = 0;
            while (nodes.length && nodes[nodes.length - 1].nodeType === 1 && nodes[nodes.length - 1].tagName === 'BR') {
                trailing += 1;
                nodes.pop();
            }
            const pieces = [];
            let bucket = [];
            const wasHeading = /^H[1-6]$/.test(block.tagName || '');
            const headingColor = (block.style && block.style.color) || '';
            const push = nodesIn => {
                const p = document.createElement('p');
                const style = block.getAttribute && block.getAttribute('style');
                if (style) {
                    const kept = filterStyle(style).split(';').map(part => part.trim())
                        .filter(part => part && !/^(color|font-weight|font-style)\s*:/i.test(part));
                    if (kept.length) p.setAttribute('style', kept.join('; '));
                }
                if (!nodesIn.length) p.appendChild(document.createElement('br'));
                nodesIn.forEach(node => p.appendChild(node));
                const text = (p.textContent || '').trim();
                if (wasHeading && isSubsectionHeading(text)) {
                    const heading = document.createElement('h3');
                    while (p.firstChild) heading.appendChild(p.firstChild);
                    heading.setAttribute('style', headingColor ? 'color: ' + headingColor + '; font-weight: 700' : 'font-weight: 700');
                    pieces.push(heading);
                    return;
                }
                pieces.push(p);
            };
            nodes.forEach(node => {
                if (node.nodeType === 1 && node.tagName === 'BR') {
                    push(bucket);
                    bucket = [];
                    return;
                }
                bucket.push(node);
            });
            push(bucket);
            for (let i = 1; i < trailing; i += 1) push([]);
            if (pieces.length < 2) return;
            if (block === root) {
                root.textContent = '';
                pieces.forEach(piece => root.appendChild(piece));
                return;
            }
            pieces.forEach(piece => block.parentNode.insertBefore(piece, block));
            block.remove();
        });
    }

    function renameTag(el, tag) {
        if (!el || el.tagName === tag.toUpperCase()) return el;
        const next = document.createElement(tag);
        [...el.attributes].forEach(attr => next.setAttribute(attr.name, attr.value));
        while (el.firstChild) next.appendChild(el.firstChild);
        el.replaceWith(next);
        return next;
    }

    function rgbOf(color) {
        const value = String(color || '').trim();
        const rgb = value.match(/rgba?\(\s*(\d+)[,\s]+(\d+)[,\s]+(\d+)/i);
        if (rgb) return [Number(rgb[1]), Number(rgb[2]), Number(rgb[3])];
        const hex = value.match(/^#([0-9a-f]{3}|[0-9a-f]{6})$/i);
        if (!hex) return null;
        let raw = hex[1];
        if (raw.length === 3) raw = raw.split('').map(ch => ch + ch).join('');
        return [parseInt(raw.slice(0, 2), 16), parseInt(raw.slice(2, 4), 16), parseInt(raw.slice(4, 6), 16)];
    }

    function isBlue(color) {
        const rgb = rgbOf(color);
        return !!rgb && rgb[2] > rgb[0] + 20 && rgb[2] > rgb[1] && rgb[0] < 90;
    }

    function plainLine(value) {
        return String(value || '').replace(/\u00a0/g, ' ').replace(/\s+/g, ' ').trim();
    }

    function isLabeledNumberLine(text) {
        return /^\d+\.\d+\s+[^:]{1,90}:\s+\S/.test(plainLine(text));
    }

    function isSubsectionHeading(text) {
        const line = plainLine(text);
        return /^\d+\.\d+\s+\S/.test(line) && !isLabeledNumberLine(line);
    }

    function emphasizeLeadIn(block) {
        if (block.querySelector('table, img')) return;
        const text = plainLine(block.textContent || '');
        const match = text.match(/^((?:\d+\.\d+\s+)?[A-Za-z][^:\uFF1A]{1,90}[:\uFF1A])(\s+\S.*)$/);
        if (!match) return;
        const partial = node => (node.textContent || '').trim().length < text.length - 8;
        if ([...block.querySelectorAll('b, strong')].some(partial)) return;
        if ([...block.querySelectorAll('[style]')].some(node => partial(node) && /^(bold|[6-9]00)$/i.test(node.style.fontWeight))) return;
        block.textContent = '';
        const strong = document.createElement('strong');
        strong.textContent = match[1];
        block.appendChild(strong);
        block.appendChild(document.createTextNode(match[2]));
    }

    function repairFlatColor(root) {
        const blocks = [...root.querySelectorAll('p, div, li, h1, h2, h3, h4, h5, h6')].filter(el =>
            !el.closest('table')
            && !el.querySelector('p, div, table, h1, h2, h3, h4, h5, h6')
            && (el.textContent || '').trim());
        if (blocks.length < 3) return;
        const blueBlocks = blocks.filter(block => {
            const colors = [];
            if (block.style.color) colors.push(block.style.color);
            block.querySelectorAll('[style]').forEach(node => { if (node.style.color) colors.push(node.style.color); });
            return colors.length > 0 && colors.every(isBlue);
        });
        if (blueBlocks.length < blocks.length * 0.7) return;

        blocks.forEach(block => {
            const text = (block.textContent || '').replace(/\s+/g, ' ').trim();
            if (isSubsectionHeading(text)) {
                const heading = renameTag(block, 'h3');
                const ownColor = heading.style.color;
                heading.style.fontWeight = '700';
                if (!ownColor) heading.style.color = '';
                heading.querySelectorAll('[style]').forEach(node => {
                    node.style.color = '';
                    node.style.fontWeight = '';
                });
                return;
            }
            const body = renameTag(block, 'p');
            const full = (body.textContent || '').trim();
            if (/^section\s+\d+\b/i.test(text)) {
                body.style.color = '#222';
                body.style.fontWeight = '700';
                body.style.fontSize = '16px';
                return;
            }
            body.style.color = '#222';
            body.style.fontWeight = '400';
            body.querySelectorAll('[style]').forEach(node => {
                if (isBlue(node.style.color)) node.style.color = '';
                const partial = (node.textContent || '').trim().length < full.length - 8;
                if (!partial && /^(bold|[6-9]00)$/i.test(node.style.fontWeight)) node.style.fontWeight = '';
            });
            emphasizeLeadIn(body);
        });
    }

    function stopInheritedHeadingColor(root) {
        [...root.querySelectorAll('p, li')].forEach(block => {
            if (!block.parentNode || block.closest('table')) return;
            if (block.querySelector('p, table, h1, h2, h3, h4, h5, h6')) return;
            const text = (block.textContent || '').replace(/\s+/g, ' ').trim();
            if (!text || block.style.color) return;
            const own = [...block.querySelectorAll('[style]')].some(node => node.style.color);
            if (own) return;
            let inherited = '';
            let parent = block.parentElement;
            while (parent) {
                if (parent.style && parent.style.color) {
                    inherited = parent.style.color;
                    break;
                }
                parent = parent.parentElement;
            }
            if (!isBlue(inherited)) return;
            if (isSubsectionHeading(text)) return;
            block.style.color = '#222';
            if (/^section\s+\d+\b/i.test(text)) block.style.fontWeight = '700';
            else emphasizeLeadIn(block);
        });
    }

    function rejoinSplitSectionNumbers(root) {
        const blocks = [...root.children];
        for (let i = 0; i < blocks.length - 1; i += 1) {
            const left = blocks[i];
            const right = blocks[i + 1];
            if (!left || !right || !left.parentNode || !right.parentNode) continue;
            if (left.closest('table') || right.closest('table')) continue;
            const lead = (left.textContent || '').replace(/\s+/g, '');
            if (!/^\d{1,2}$/.test(lead)) continue;
            const follow = (right.textContent || '').replace(/^\s+/, '');
            if (!/^0\.\d+\s+\S/.test(follow)) continue;
            const textNode = document.createTreeWalker(right, NodeFilter.SHOW_TEXT).nextNode();
            if (textNode) textNode.textContent = lead + String(textNode.textContent || '').replace(/^\s*/, '');
            else right.insertBefore(document.createTextNode(lead), right.firstChild);
            left.remove();
        }
    }

    function repairNumberedLabels(root) {
        [...root.querySelectorAll('p, li, h1, h2, h3, h4, h5, h6')].forEach(block => {
            if (!block.parentNode || block.closest('table') || block.querySelector('table')) return;
            const text = plainLine(block.textContent || '');
            if (!isLabeledNumberLine(text)) return;
            const body = renameTag(block, 'p');
            body.style.fontWeight = '';
            body.style.fontSize = '';
            body.querySelectorAll('[style], font').forEach(node => {
                node.style.color = '';
                if (node.tagName === 'FONT') node.removeAttribute('color');
                const partial = (node.textContent || '').trim().length < text.length - 8;
                if (!partial && /^(bold|[6-9]00)$/i.test(node.style.fontWeight)) node.style.fontWeight = '';
            });
            emphasizeLeadIn(body);
            body.style.color = '#222';
            body.style.fontWeight = '400';
        });
    }

    function keepLeadInBoldOnly(root) {
        const lead = /^((?:\d+\.\d+\s+)?[A-Za-z][^:\uFF1A]{1,90}[:\uFF1A])(\s+\S.*)$/;
        [...root.querySelectorAll('p, li, div, h1, h2, h3, h4, h5, h6')].forEach(block => {
            if (!block.parentNode || block.closest('table')) return;
            if (block.querySelector('p, div, table, ul, ol, h1, h2, h3, h4, h5, h6')) return;
            const text = plainLine(block.textContent || '');
            const match = text.match(lead);
            if (!match) return;
            const body = /^(H[1-6]|DIV)$/.test(block.tagName) ? renameTag(block, 'p') : block;
            const labelLength = match[1].length;
            body.querySelectorAll('b, strong').forEach(node => {
                if (plainLine(node.textContent || '').length > labelLength + 1)
                    node.replaceWith(...node.childNodes);
            });
            body.querySelectorAll('[style]').forEach(node => {
                if (!/^(bold|[6-9]00)$/i.test(node.style.fontWeight)) return;
                if (plainLine(node.textContent || '').length > labelLength + 1) node.style.fontWeight = '';
            });
            emphasizeLeadIn(body);
            body.style.fontWeight = '400';
        });
    }

    function sanitizeHtml(html) {
        const root = document.createElement('div');
        root.innerHTML = cleanWordHtml(inlineClipboardCss(html));
        sanitizeNode(root);
        splitGluedHeadings(root);
        promoteBreaks(root);
        rejoinSplitSectionNumbers(root);
        repairFlatColor(root);
        stopInheritedHeadingColor(root);
        repairNumberedLabels(root);
        keepLeadInBoldOnly(root);
        return root.innerHTML;
    }

    function hasRealFormatting(root) {
        return [...root.querySelectorAll('*')].some(el => {
            if (el.tagName === 'BR') return false;
            if ((el.getAttribute('style') || '').trim()) return true;
            return el.tagName !== 'DIV' && el.tagName !== 'P' && el.tagName !== 'SPAN';
        });
    }

    function plainFrom(root) {
        let out = '';
        const walk = node => {
            node.childNodes.forEach(child => {
                if (child.nodeType === Node.TEXT_NODE) {
                    out += child.textContent || '';
                    return;
                }
                if (child.nodeType !== Node.ELEMENT_NODE) return;
                if (child.tagName === 'BR') {
                    out += '\n';
                    return;
                }
                const block = child.tagName === 'DIV' || child.tagName === 'P' || child.tagName === 'LI';
                if (block && out && !out.endsWith('\n')) out += '\n';
                walk(child);
            });
        };
        walk(root);
        return out.replace(/\u200b/g, '').replace(/\u00a0/g, ' ').replace(/\n{3,}/g, '\n\n').trim();
    }

    function optionList(values, selected) {
        return values.map(v => `<option value="${v}"${v === selected ? ' selected' : ''}>${v}</option>`).join('');
    }

    const HIGHLIGHTS = ['#FFFF00', '#00FF00', '#00FFFF', '#FF00FF', '#0000FF', '#FF0000', '#800000', '#008080', '#808000', '#808080', '#000000'];
    const THEME_COLORS = [
        ['#FFFFFF', '#000000', '#E7E6E6', '#44546A', '#4472C4', '#ED7D31', '#A5A5A5', '#FFC000', '#5B9BD5', '#70AD47'],
        ['#F2F2F2', '#7F7F7F', '#D0CECE', '#D6DCE4', '#D6E2F4', '#FCE4D6', '#EDEDED', '#FFF2CC', '#DDEBF7', '#E2EFDA'],
        ['#D8D8D8', '#595959', '#AEAAAA', '#ADB9CA', '#B4C6E7', '#F8CBAD', '#DBDBDB', '#FFE699', '#BDD7EE', '#C6E0B4'],
        ['#BFBFBF', '#3F3F3F', '#757070', '#8496B0', '#8FAADC', '#F4B183', '#C9C9C9', '#FFD966', '#9DC3E6', '#A9D08E'],
        ['#A6A6A6', '#262626', '#3A3838', '#323F4F', '#2F5496', '#C65911', '#7B7B7B', '#BF8F00', '#2E75B6', '#548235'],
        ['#7F7F7F', '#0C0C0C', '#171616', '#222A35', '#1F3864', '#833C0C', '#525252', '#806000', '#1E4E79', '#375623']
    ];
    const STANDARD_COLORS = ['#C00000', '#FF0000', '#FFC000', '#FFFF00', '#92D050', '#00B050', '#00B0F0', '#0070C0', '#002060', '#7030A0'];

    function swatchHtml(colors, kind) {
        return colors.map(color => `<button type="button" class="word-swatch" style="background:${color}" data-swatch="${color}" data-kind="${kind}" title="${color}"></button>`).join('');
    }

    function buildRibbon() {
        const ribbon = document.createElement('div');
        ribbon.className = 'word-ribbon';
        ribbon.setAttribute('role', 'toolbar');
        ribbon.setAttribute('aria-label', 'Description formatting');
        ribbon.innerHTML = `
            <div class="word-group">
                <div class="word-group-tools word-clip">
                    <button type="button" class="word-btn word-btn-lg" data-cmd="paste" title="Paste"><i class="fas fa-paste"></i><span>Paste</span></button>
                    <div class="word-clip-stack">
                        <button type="button" class="word-btn word-btn-sm" data-cmd="cut" title="Cut"><i class="fas fa-scissors"></i><span>Cut</span></button>
                        <button type="button" class="word-btn word-btn-sm" data-cmd="copy" title="Copy"><i class="fas fa-copy"></i><span>Copy</span></button>
                        <button type="button" class="word-btn word-btn-sm" data-cmd="painter" title="Format Painter"><i class="fas fa-paintbrush"></i><span>Format Painter</span></button>
                    </div>
                </div>
                <div class="word-group-caption">Clipboard</div>
            </div>
            <div class="word-group">
                <div class="word-group-tools word-font">
                    <div class="word-font-row">
                        <select class="word-select word-font-family" title="Font" data-cmd="fontName">${optionList(FONTS, 'Times New Roman')}</select>
                        <select class="word-select word-font-size" title="Font size" data-cmd="fontSize">${optionList(SIZES, 12)}</select>
                        <button type="button" class="word-btn" data-cmd="grow" title="Grow font"><span class="word-a-grow">A</span></button>
                        <button type="button" class="word-btn" data-cmd="shrink" title="Shrink font"><span class="word-a-shrink">A</span></button>
                        <div class="word-pop">
                            <button type="button" class="word-btn" data-menu="case" title="Change case"><span class="word-aa">Aa</span></button>
                            <div class="word-menu" hidden>
                                <button type="button" data-case="sentence">Sentence case.</button>
                                <button type="button" data-case="lower">lowercase</button>
                                <button type="button" data-case="upper">UPPERCASE</button>
                                <button type="button" data-case="capitalize">Capitalize Each Word</button>
                            </div>
                        </div>
                        <button type="button" class="word-btn" data-cmd="clear" title="Clear formatting"><i class="fas fa-eraser"></i></button>
                    </div>
                    <div class="word-font-row">
                        <button type="button" class="word-btn" data-cmd="bold" data-state="bold" title="Bold"><i class="fas fa-bold"></i></button>
                        <button type="button" class="word-btn" data-cmd="italic" data-state="italic" title="Italic"><i class="fas fa-italic"></i></button>
                        <button type="button" class="word-btn" data-cmd="underline" data-state="underline" title="Underline"><i class="fas fa-underline"></i></button>
                        <button type="button" class="word-btn" data-cmd="strikeThrough" data-state="strikeThrough" title="Strikethrough"><i class="fas fa-strikethrough"></i></button>
                        <button type="button" class="word-btn" data-cmd="subscript" data-state="subscript" title="Subscript"><i class="fas fa-subscript"></i></button>
                        <button type="button" class="word-btn" data-cmd="superscript" data-state="superscript" title="Superscript"><i class="fas fa-superscript"></i></button>
                        <div class="word-pop">
                            <button type="button" class="word-btn word-color" data-menu="highlight" title="Text highlight color">
                                <i class="fas fa-highlighter"></i>
                                <span class="word-color-bar" data-bar="highlight"></span>
                            </button>
                            <div class="word-menu word-color-menu word-highlight-menu" hidden>
                                <div class="word-swatch-grid word-highlight-grid">${swatchHtml(HIGHLIGHTS.slice(0, 5), 'highlight')}</div>
                                <div class="word-swatch-grid word-highlight-grid word-highlight-grid-6">${swatchHtml(HIGHLIGHTS.slice(5), 'highlight')}</div>
                                <button type="button" class="word-color-action" data-swatch="none" data-kind="highlight">No Color</button>
                                <button type="button" class="word-color-action" data-highlight-stop="1">Stop Highlighting</button>
                            </div>
                        </div>
                        <div class="word-pop">
                            <button type="button" class="word-btn word-color" data-menu="font" title="Font color">
                                <span class="word-color-a">A</span>
                                <span class="word-color-bar" data-bar="font"></span>
                            </button>
                            <div class="word-menu word-color-menu" hidden>
                                <button type="button" class="word-color-action" data-swatch="#222222" data-kind="font"><i class="word-auto-chip"></i> Automatic</button>
                                <div class="word-color-label">Theme Colors</div>
                                ${THEME_COLORS.map(row => `<div class="word-swatch-grid">${swatchHtml(row, 'font')}</div>`).join('')}
                                <div class="word-color-label">Standard Colors</div>
                                <div class="word-swatch-grid">${swatchHtml(STANDARD_COLORS, 'font')}</div>
                                <button type="button" class="word-color-action" data-more="font"><i class="fas fa-palette"></i> More Colors...</button>
                                <input type="color" class="word-more-input" data-kind="font" value="#c00000" hidden>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="word-group-caption">Font</div>
            </div>
            <div class="word-group">
                <div class="word-group-tools word-font">
                    <div class="word-font-row">
                        <button type="button" class="word-btn" data-cmd="insertUnorderedList" data-state="insertUnorderedList" title="Bullets"><i class="fas fa-list-ul"></i></button>
                        <button type="button" class="word-btn" data-cmd="insertOrderedList" data-state="insertOrderedList" title="Numbering"><i class="fas fa-list-ol"></i></button>
                        <button type="button" class="word-btn" data-cmd="outdent" title="Decrease indent"><i class="fas fa-outdent"></i></button>
                        <button type="button" class="word-btn" data-cmd="indent" title="Increase indent"><i class="fas fa-indent"></i></button>
                    </div>
                    <div class="word-font-row">
                        <button type="button" class="word-btn" data-cmd="justifyLeft" data-state="justifyLeft" title="Align left"><i class="fas fa-align-left"></i></button>
                        <button type="button" class="word-btn" data-cmd="justifyCenter" data-state="justifyCenter" title="Center"><i class="fas fa-align-center"></i></button>
                        <button type="button" class="word-btn" data-cmd="justifyRight" data-state="justifyRight" title="Align right"><i class="fas fa-align-right"></i></button>
                        <button type="button" class="word-btn" data-cmd="justifyFull" data-state="justifyFull" title="Justify"><i class="fas fa-align-justify"></i></button>
                        <div class="word-pop">
                            <button type="button" class="word-btn" data-menu="spacing" title="Line spacing"><i class="fas fa-text-height"></i></button>
                            <div class="word-menu" hidden>
                                <button type="button" data-space="1">1.0</button>
                                <button type="button" data-space="1.15">1.15</button>
                                <button type="button" data-space="1.5">1.5</button>
                                <button type="button" data-space="2">2.0</button>
                                <button type="button" data-space="2.5">2.5</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="word-group-caption">Paragraph</div>
            </div>
            <div class="word-group">
                <div class="word-group-tools word-font word-table-actions">
                    <div class="word-font-row">
                        <div class="word-pop">
                            <button type="button" class="word-btn" data-menu="table" title="Insert table"><i class="fas fa-table"></i><span class="word-btn-text">Table</span></button>
                            <div class="word-menu word-table-menu" hidden>
                                <label>Rows <input type="number" class="word-table-rows" min="1" max="20" value="3"></label>
                                <label>Columns <input type="number" class="word-table-cols" min="1" max="12" value="2"></label>
                                <button type="button" class="word-table-insert" data-cmd="insertTable">Insert table</button>
                            </div>
                        </div>
                        <button type="button" class="word-btn" data-cmd="addRow" title="Add row"><i class="fas fa-plus"></i><span class="word-btn-text">Row</span></button>
                        <button type="button" class="word-btn" data-cmd="addCol" title="Add column"><i class="fas fa-plus"></i><span class="word-btn-text">Column</span></button>
                        <button type="button" class="word-btn" data-cmd="delRow" title="Delete row"><i class="fas fa-minus"></i><span class="word-btn-text">Row</span></button>
                        <button type="button" class="word-btn" data-cmd="delCol" title="Delete column"><i class="fas fa-minus"></i><span class="word-btn-text">Column</span></button>
                        <button type="button" class="word-btn" data-cmd="mergeCells" title="Merge selected cells"><i class="fas fa-object-group"></i><span class="word-btn-text">Merge</span></button>
                    </div>
                    <div class="word-font-row">
                        <button type="button" class="word-btn" data-cmd="tableSmaller" title="Make table smaller"><i class="fas fa-compress"></i><span class="word-btn-text">Smaller</span></button>
                        <button type="button" class="word-btn" data-cmd="tableBigger" title="Make table bigger"><i class="fas fa-expand"></i><span class="word-btn-text">Bigger</span></button>
                        <label class="word-btn word-image-btn" title="Upload image">
                            <i class="fas fa-image"></i><span class="word-btn-text">Image</span>
                            <input type="file" class="word-image-input" accept="image/png,image/jpeg,image/gif,image/webp" hidden>
                        </label>
                    </div>
                </div>
                <div class="word-group-caption">Table</div>
            </div>`;
        return ribbon;
    }

    function attach(textarea, onChange) {
        if (!textarea || textarea.dataset.wordEditor === '1') return;
        textarea.dataset.wordEditor = '1';

        const wrap = document.createElement('div');
        wrap.className = 'word-editor-wrap';
        textarea.parentNode.insertBefore(wrap, textarea);

        const ribbon = buildRibbon();
        const surface = document.createElement('div');
        surface.className = 'word-surface is-empty';
        surface.contentEditable = 'true';
        surface.spellcheck = true;
        surface.setAttribute('role', 'textbox');
        surface.setAttribute('aria-multiline', 'true');
        surface.dataset.placeholder = textarea.getAttribute('placeholder') || 'Text for this subsection';

        wrap.appendChild(ribbon);
        wrap.appendChild(surface);
        wrap.appendChild(textarea);
        textarea.classList.add('d-none');

        const fontSelect = ribbon.querySelector('.word-font-family');
        const sizeSelect = ribbon.querySelector('.word-font-size');
        const fontBar = ribbon.querySelector('[data-bar="font"]');
        const highlightBar = ribbon.querySelector('[data-bar="highlight"]');
        fontBar.style.background = '#c00000';
        highlightBar.style.background = '#fff59d';

        let savedRange = null;
        let paintStyle = null;

        function loadInitial(value) {
            const raw = value || '';
            if (!raw.trim()) {
                surface.innerHTML = '';
                surface.classList.add('is-empty');
                return;
            }
            const looksMarkedUp = /<[a-z][\s\S]*>/i.test(raw) || /StartFragment|@font-face|<style/i.test(raw);
            if (looksMarkedUp) surface.innerHTML = sanitizeHtml(raw);
            else surface.innerHTML = escapeHtml(stripOfficeJunk(raw)).replace(/\n/g, '<br>');
            refreshEmpty();
        }

        function refreshEmpty() {
            const text = (surface.innerText || '').replace(/\u200b/g, '').trim();
            surface.classList.toggle('is-empty', !text);
        }

        function snapshotHtml() {
            const clone = surface.cloneNode(true);
            clone.querySelectorAll('.word-table-grip').forEach(el => el.remove());
            clone.querySelectorAll('.word-image-resize').forEach(el => el.remove());
            clone.querySelectorAll('.word-table-frame').forEach(frame => {
                const parent = frame.parentNode;
                while (frame.firstChild) parent.insertBefore(frame.firstChild, frame);
                frame.remove();
            });
            return sanitizeHtml(clone.innerHTML);
        }

        function emit() {
            const clean = snapshotHtml();
            const holder = document.createElement('div');
            holder.innerHTML = clean;
            const text = plainFrom(holder);
            textarea.value = !text ? '' : (hasRealFormatting(holder) ? clean : text);
            refreshEmpty();
            decorateTables();
            if (typeof onChange === 'function') onChange();
        }

        const undoStack = [];
        const redoStack = [];
        let historyLock = false;

        function checkpoint() {
            if (historyLock) return;
            const html = surface.innerHTML;
            if (undoStack[undoStack.length - 1] === html) return;
            undoStack.push(html);
            if (undoStack.length > 100) undoStack.shift();
            redoStack.length = 0;
        }

        function restoreHtml(html) {
            historyLock = true;
            surface.innerHTML = html;
            savedRange = null;
            selectedImage = null;
            if (imageHandle) imageHandle.remove();
            imageHandle = null;
            historyLock = false;
            emit();
        }

        function undoEdit() {
            const current = surface.innerHTML;
            if (undoStack[undoStack.length - 1] === current) undoStack.pop();
            const prev = undoStack.pop();
            if (prev == null) return;
            redoStack.push(current);
            restoreHtml(prev);
        }

        function redoEdit() {
            const next = redoStack.pop();
            if (next == null) return;
            const current = surface.innerHTML;
            if (undoStack[undoStack.length - 1] !== current) undoStack.push(current);
            restoreHtml(next);
        }

        function chosenCells() {
            return [...surface.querySelectorAll('td.is-cell-selected, th.is-cell-selected')];
        }

        function isTableRow(el) {
            if (!el || el === surface) return false;
            if (el.classList.contains('word-table-row')) return true;
            const style = el.getAttribute('style') || '';
            return /display\s*:\s*flex/i.test(style) && !!el.querySelector('table');
        }

        function releaseRow(row) {
            if (!row || !isTableRow(row)) return;
            const frames = [...row.children].filter(child => child.classList.contains('word-table-frame'));
            if (frames.length > 1) return;
            while (row.firstChild) row.before(row.firstChild);
            row.remove();
        }

        function placeBeside(sourceFrame, targetFrame, side) {
            if (!sourceFrame || !targetFrame || sourceFrame === targetFrame) return;
            const oldRow = isTableRow(sourceFrame.parentElement) ? sourceFrame.parentElement : null;
            let row = isTableRow(targetFrame.parentElement) ? targetFrame.parentElement : null;
            if (!row) {
                row = document.createElement('div');
                row.className = 'word-table-row';
                row.setAttribute('style', 'display: flex; align-items: flex-start; gap: 16px; flex-wrap: wrap;');
                targetFrame.before(row);
                row.appendChild(targetFrame);
            }
            if (side === 'before') targetFrame.before(sourceFrame);
            else targetFrame.after(sourceFrame);
            [sourceFrame, targetFrame].forEach(frame => {
                const table = frame.querySelector('table');
                if (!table) return;
                const width = Math.round(table.getBoundingClientRect().width);
                if (width > 40) table.style.width = width + 'px';
            });
            if (oldRow && oldRow !== row) releaseRow(oldRow);
        }

        function clearDropMarks() {
            surface.querySelectorAll('.word-table-frame').forEach(frame => {
                frame.classList.remove('is-drop-left', 'is-drop-right', 'is-dragging');
            });
        }

        let tableDrag = null;

        function startTableDrag(frame, event) {
            if (event.button !== undefined && event.button !== 0) return;
            event.preventDefault();
            event.stopPropagation();
            const rect = frame.getBoundingClientRect();
            const placeholder = document.createElement('div');
            placeholder.className = 'word-table-placeholder';
            placeholder.style.width = rect.width + 'px';
            placeholder.style.height = rect.height + 'px';
            frame.before(placeholder);
            frame.classList.add('is-dragging');
            frame.style.position = 'fixed';
            frame.style.left = rect.left + 'px';
            frame.style.top = rect.top + 'px';
            frame.style.width = rect.width + 'px';
            frame.style.margin = '0';
            frame.style.zIndex = '80';
            tableDrag = {
                frame,
                placeholder,
                offsetX: event.clientX - rect.left,
                offsetY: event.clientY - rect.top
            };
            try { event.currentTarget.setPointerCapture(event.pointerId); } catch { /* pointer already captured */ }
            document.addEventListener('pointermove', onTableDrag);
            document.addEventListener('pointerup', stopTableDrag);
            document.addEventListener('pointercancel', stopTableDrag);
        }

        function onTableDrag(event) {
            if (!tableDrag) return;
            event.preventDefault();
            const frame = tableDrag.frame;
            frame.style.left = (event.clientX - tableDrag.offsetX) + 'px';
            frame.style.top = (event.clientY - tableDrag.offsetY) + 'px';
            clearDropMarks();
            frame.classList.add('is-dragging');
            tableDrag.target = null;
            tableDrag.side = null;
            const editor = surface.getBoundingClientRect();
            let best = null;
            surface.querySelectorAll('.word-table-frame').forEach(other => {
                if (other === frame) return;
                const rect = other.getBoundingClientRect();
                const inBand = event.clientY >= rect.top - 30 && event.clientY <= rect.bottom + 36;
                const inReach = event.clientX >= rect.left - 20 && event.clientX <= editor.right + 8;
                if (!inBand || !inReach) return;
                const score = Math.abs(event.clientY - (rect.top + rect.height / 2));
                if (!best || score < best.score) {
                    best = {
                        frame: other,
                        score,
                        side: event.clientX > rect.left + rect.width * 0.35 ? 'after' : 'before'
                    };
                }
            });
            if (!best) return;
            best.frame.classList.add(best.side === 'after' ? 'is-drop-right' : 'is-drop-left');
            tableDrag.target = best.frame;
            tableDrag.side = best.side;
        }

        function stopTableDrag() {
            if (!tableDrag) return;
            const drag = tableDrag;
            tableDrag = null;
            document.removeEventListener('pointermove', onTableDrag);
            document.removeEventListener('pointerup', stopTableDrag);
            document.removeEventListener('pointercancel', stopTableDrag);
            drag.frame.classList.remove('is-dragging');
            drag.frame.style.position = '';
            drag.frame.style.left = '';
            drag.frame.style.top = '';
            drag.frame.style.width = '';
            drag.frame.style.margin = '';
            drag.frame.style.zIndex = '';
            drag.placeholder.remove();
            clearDropMarks();
            if (drag.target && drag.side) placeBeside(drag.frame, drag.target, drag.side);
            emit();
        }

        function selectFrame(frame) {
            surface.querySelectorAll('.word-table-frame.is-selected').forEach(other => {
                if (other !== frame) other.classList.remove('is-selected');
            });
            if (frame) frame.classList.add('is-selected');
        }

        function decorateTables() {
            surface.querySelectorAll('table').forEach(table => {
                let frame = table.parentElement;
                if (!frame || !frame.classList.contains('word-table-frame')) {
                    frame = document.createElement('div');
                    frame.className = 'word-table-frame';
                    table.before(frame);
                    frame.appendChild(table);
                }
                if (isTableRow(frame.parentElement)) frame.parentElement.classList.add('word-table-row');
                if (frame.querySelector(':scope > .word-table-grip')) return;
                const grip = document.createElement('div');
                grip.className = 'word-table-grip';
                grip.contentEditable = 'false';
                grip.title = 'Drag this table beside another table';
                grip.innerHTML = '<i class="fas fa-grip-lines"></i><span>Drag</span>';
                frame.insertBefore(grip, frame.firstChild);
                grip.addEventListener('pointerdown', event => startTableDrag(frame, event));
            });
        }

        surface.addEventListener('pointerdown', event => {
            if (tableDrag || event.target.closest('.word-table-grip') || event.target.closest('.word-image-resize')) return;
            const img = event.target.closest && event.target.closest('img');
            if (img && surface.contains(img)) {
                event.preventDefault();
                selectImage(img);
                selectFrame(null);
                return;
            }
            selectImage(null);
            const table = event.target.closest && event.target.closest('table');
            const frame = table && surface.contains(table) ? table.closest('.word-table-frame') : null;
            selectFrame(frame);
        });

        let selectedImage = null;
        let imageHandle = null;

        function placeImageHandle() {
            if (!imageHandle || !selectedImage || !selectedImage.isConnected) return;
            const surfaceRect = surface.getBoundingClientRect();
            const rect = selectedImage.getBoundingClientRect();
            imageHandle.style.left = (rect.right - surfaceRect.left + surface.scrollLeft - 8) + 'px';
            imageHandle.style.top = (rect.bottom - surfaceRect.top + surface.scrollTop - 8) + 'px';
        }

        function selectImage(img) {
            surface.querySelectorAll('img.is-selected').forEach(other => {
                if (other !== img) other.classList.remove('is-selected');
            });
            selectedImage = img && surface.contains(img) ? img : null;
            if (!selectedImage) {
                imageHandle?.remove();
                imageHandle = null;
                return;
            }
            selectedImage.classList.add('is-selected');
            if (!imageHandle) {
                imageHandle = document.createElement('div');
                imageHandle.className = 'word-image-resize';
                imageHandle.contentEditable = 'false';
                imageHandle.title = 'Drag to resize image';
                surface.appendChild(imageHandle);
                imageHandle.addEventListener('pointerdown', startImageResize);
            }
            placeImageHandle();
        }

        function scaleImage(factor) {
            if (!selectedImage) return false;
            const current = selectedImage.getBoundingClientRect().width;
            const max = Math.max(120, surface.clientWidth - 16);
            const next = Math.max(80, Math.min(max, Math.round(current * factor)));
            selectedImage.style.width = next + 'px';
            selectedImage.style.height = 'auto';
            placeImageHandle();
            emit();
            return true;
        }

        function startImageResize(event) {
            if (!selectedImage) return;
            event.preventDefault();
            event.stopPropagation();
            const startX = event.clientX;
            const startWidth = selectedImage.getBoundingClientRect().width;
            const move = ev => {
                const max = Math.max(120, surface.clientWidth - 16);
                const next = Math.max(80, Math.min(max, Math.round(startWidth + (ev.clientX - startX))));
                selectedImage.style.width = next + 'px';
                selectedImage.style.height = 'auto';
                placeImageHandle();
            };
            const up = () => {
                document.removeEventListener('pointermove', move);
                document.removeEventListener('pointerup', up);
                emit();
                placeImageHandle();
            };
            try { event.currentTarget.setPointerCapture(event.pointerId); } catch { /* ignore */ }
            document.addEventListener('pointermove', move);
            document.addEventListener('pointerup', up);
        }

        surface.addEventListener('scroll', () => placeImageHandle());

        function inSurface(node) {
            return !!node && (node === surface || surface.contains(node));
        }

        const BLOCK_TAG = /^(P|DIV|H[1-6]|LI)$/;

        function topBlock(node) {
            let el = node && node.nodeType === 1 ? node : node && node.parentElement;
            let block = null;
            while (el && el !== surface) {
                if (el.tagName === 'TD' || el.tagName === 'TH' || el.tagName === 'TABLE') return null;
                if (BLOCK_TAG.test(el.tagName) && !el.classList.contains('word-table-frame') && !el.classList.contains('word-table-row'))
                    block = el;
                el = el.parentElement;
            }
            return block;
        }

        function atBlockStart(block, range) {
            const probe = document.createRange();
            probe.setStart(block, 0);
            probe.setEnd(range.startContainer, range.startOffset);
            return probe.toString().replace(/\u200b/g, '').length === 0
                && !probe.cloneContents().querySelector('br, img, table');
        }

        function previousBlock(block) {
            let node = block.previousSibling;
            while (node) {
                if (node.nodeType === 1 && BLOCK_TAG.test(node.tagName)) return node;
                if (node.nodeType === 3 && node.textContent.replace(/\u200b/g, '').trim()) return null;
                node = node.previousSibling;
            }
            return null;
        }

        function isBlankBlock(el) {
            if (!el || el.querySelector('img, table')) return false;
            return !(el.textContent || '').replace(/\u200b/g, '').replace(/\u00a0/g, ' ').trim();
        }

        function isSectionHeading(block) {
            if (/^H[1-6]$/.test(block.tagName)) return true;
            return /^\d+\.\d+\s+\S/.test((block.innerText || '').replace(/\u200b/g, '').trim());
        }

        function lastLineHasRoom(el) {
            const host = surface.getBoundingClientRect();
            const pad = parseFloat(getComputedStyle(surface).paddingRight) || 0;
            const limit = host.right - pad - 12;
            const range = document.createRange();
            range.selectNodeContents(el);
            const rects = [...range.getClientRects()].filter(rect => rect.width > 0 && rect.height > 0);
            if (!rects.length) return true;
            return (limit - rects[rects.length - 1].right) > 20;
        }

        surface.addEventListener('beforeinput', event => {
            if (historyLock) return;
            if (event.inputType === 'historyUndo' || event.inputType === 'historyRedo') {
                event.preventDefault();
                return;
            }
            checkpoint();
        });

        surface.addEventListener('keydown', event => {
            if (event.key !== 'Backspace' || event.altKey || event.ctrlKey || event.metaKey) return;
            const sel = window.getSelection();
            if (!sel || !sel.isCollapsed || !sel.rangeCount) return;
            const range = sel.getRangeAt(0);
            if (!inSurface(range.startContainer)) return;
            const block = topBlock(range.startContainer);
            if (!block || !atBlockStart(block, range) || !isSectionHeading(block)) return;
            const prev = previousBlock(block);
            if (!prev) return;
            if (isBlankBlock(prev)) {
                event.preventDefault();
                prev.remove();
                emit();
                return;
            }
            if (lastLineHasRoom(prev)) event.preventDefault();
        });

        function saveSelection() {
            const sel = window.getSelection();
            if (!sel || !sel.rangeCount) return;
            const range = sel.getRangeAt(0);
            if (inSurface(range.commonAncestorContainer)) savedRange = range.cloneRange();
        }

        function restoreSelection() {
            if (!savedRange) return;
            const range = savedRange.cloneRange();
            surface.focus();
            const sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(range);
        }

        function paintElement(el, kind, value) {
            if (kind === 'font') {
                el.style.color = value;
                if (el.tagName === 'FONT') el.setAttribute('color', value);
                return;
            }
            if (value === 'transparent') {
                el.style.backgroundColor = '';
                el.style.background = '';
                return;
            }
            el.style.backgroundColor = value;
        }

        function paintChosenCells(cells, kind, value) {
            checkpoint();
            cells.forEach(cell => {
                paintElement(cell, kind, value);
                cell.querySelectorAll('span, font, p, b, strong, i, em, u, s, li').forEach(el => paintElement(el, kind, value));
            });
            emit();
        }

        function cellsTouched(range) {
            const cells = [];
            surface.querySelectorAll('td, th').forEach(cell => {
                try {
                    if (range.intersectsNode(cell)) cells.push(cell);
                } catch { /* detached */ }
            });
            return cells;
        }

        function paintSelection(kind, value) {
            const chosen = chosenCells();
            if (chosen.length) {
                paintChosenCells(chosen, kind, value);
                return;
            }
            restoreSelection();
            const sel = window.getSelection();
            if (!sel || !sel.rangeCount || !inSurface(sel.getRangeAt(0).commonAncestorContainer)) return;
            const range = sel.getRangeAt(0);
            if (range.collapsed) {
                checkpoint();
                document.execCommand('styleWithCSS', false, true);
                document.execCommand(kind === 'font' ? 'foreColor' : 'hiliteColor', false, value);
                saveSelection();
                emit();
                return;
            }
            const touched = cellsTouched(range);
            if (touched.length > 1) {
                paintChosenCells(touched, kind, value);
                return;
            }
            checkpoint();
            const span = document.createElement('span');
            if (kind === 'font') span.style.color = value;
            else if (value !== 'transparent') span.style.backgroundColor = value;
            const frag = range.extractContents();
            frag.querySelectorAll('span, font').forEach(el => {
                if (kind === 'font') {
                    el.style.color = '';
                    if (el.tagName === 'FONT') el.removeAttribute('color');
                } else {
                    el.style.backgroundColor = '';
                    el.style.background = '';
                }
            });
            if (kind === 'highlight' && value === 'transparent') {
                range.insertNode(frag);
            } else {
                span.appendChild(frag);
                range.insertNode(span);
                const next = document.createRange();
                next.selectNodeContents(span);
                sel.removeAllRanges();
                sel.addRange(next);
                savedRange = next.cloneRange();
            }
            emit();
        }

        function exec(command, value) {
            checkpoint();
            restoreSelection();
            document.execCommand('styleWithCSS', false, true);
            document.execCommand(command, false, value);
            saveSelection();
            emit();
        }

        function toggleWrap(tag) {
            const chosen = chosenCells();
            checkpoint();
            restoreSelection();
            const sel = window.getSelection();
            const live = sel && sel.rangeCount ? sel.getRangeAt(0) : null;
            const hasText = !!(live && !live.collapsed && inSurface(live.commonAncestorContainer) && live.toString());
            if (chosen.length && !hasText) {
                const prop = tag === 'B' ? 'fontWeight' : tag === 'I' ? 'fontStyle' : 'textDecorationLine';
                const on = tag === 'B' ? 'bold' : tag === 'I' ? 'italic' : tag === 'S' ? 'line-through' : 'underline';
                chosen.forEach(cell => { cell.style[prop] = cell.style[prop] === on ? '' : on; });
                emit();
                return;
            }
            if (!live || !inSurface(live.commonAncestorContainer)) return;
            if (live.collapsed) {
                const command = tag === 'B' ? 'bold' : tag === 'I' ? 'italic' : tag === 'S' ? 'strikeThrough' : 'underline';
                document.execCommand(command, false, null);
                saveSelection();
                emit();
                return;
            }
            let node = live.commonAncestorContainer.nodeType === 1 ? live.commonAncestorContainer : live.commonAncestorContainer.parentElement;
            const existing = node && node.closest ? node.closest(tag) : null;
            if (existing && existing !== surface && surface.contains(existing)) {
                const parent = existing.parentNode;
                while (existing.firstChild) parent.insertBefore(existing.firstChild, existing);
                existing.remove();
                emit();
                return;
            }
            const el = document.createElement(tag);
            el.appendChild(live.extractContents());
            live.insertNode(el);
            const next = document.createRange();
            next.selectNodeContents(el);
            sel.removeAllRanges();
            sel.addRange(next);
            savedRange = next.cloneRange();
            emit();
        }

        function currentPointSize() {
            const cell = surface.querySelector('td.is-cell-selected, th.is-cell-selected');
            let node = cell || null;
            if (!node) {
                const sel = window.getSelection();
                node = sel && sel.anchorNode;
                if (node && node.nodeType === Node.TEXT_NODE) node = node.parentElement;
            }
            if (!inSurface(node)) return 12;
            const px = parseFloat(getComputedStyle(node).fontSize);
            if (!px) return 12;
            return Math.round(px * 0.75);
        }

        function applyFontSize(pt) {
            const cells = chosenCells();
            if (cells.length) {
                checkpoint();
                cells.forEach(cell => {
                    cell.style.fontSize = pt + 'pt';
                    cell.querySelectorAll('span, font').forEach(el => { el.style.fontSize = ''; });
                });
                sizeSelect.value = String(pt);
                emit();
                return;
            }
            checkpoint();
            restoreSelection();
            const sel = window.getSelection();
            if (!sel || !sel.rangeCount) return;
            const range = sel.getRangeAt(0);
            if (!inSurface(range.commonAncestorContainer)) return;
            const span = document.createElement('span');
            span.style.fontSize = pt + 'pt';
            if (range.collapsed) {
                span.textContent = '\u200b';
                range.insertNode(span);
                const caret = document.createRange();
                caret.setStart(span.firstChild, 1);
                caret.collapse(true);
                sel.removeAllRanges();
                sel.addRange(caret);
                savedRange = caret.cloneRange();
            } else {
                span.appendChild(range.extractContents());
                range.insertNode(span);
                const next = document.createRange();
                next.selectNodeContents(span);
                sel.removeAllRanges();
                sel.addRange(next);
                savedRange = next.cloneRange();
            }
            sizeSelect.value = String(pt);
            emit();
        }

        function bumpSize(dir) {
            const current = currentPointSize();
            let next = current;
            if (dir > 0) next = SIZES.find(size => size > current + 0.25) || SIZES[SIZES.length - 1];
            else next = [...SIZES].reverse().find(size => size < current - 0.25) || SIZES[0];
            applyFontSize(next);
        }

        function changeCase(mode) {
            restoreSelection();
            const sel = window.getSelection();
            if (!sel || !sel.rangeCount || sel.isCollapsed) return;
            const range = sel.getRangeAt(0);
            const text = range.toString();
            if (!text) return;
            let next = text;
            if (mode === 'upper') next = text.toUpperCase();
            else if (mode === 'lower') next = text.toLowerCase();
            else if (mode === 'capitalize') next = text.replace(/\S+/g, word => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase());
            else next = text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
            range.deleteContents();
            range.insertNode(document.createTextNode(next));
            emit();
        }

        function closestBlock() {
            const sel = window.getSelection();
            let node = sel && sel.anchorNode;
            if (!node) return null;
            if (node.nodeType === Node.TEXT_NODE) node = node.parentElement;
            while (node && node !== surface) {
                if (node.tagName === 'P' || node.tagName === 'DIV' || node.tagName === 'LI') return node;
                node = node.parentElement;
            }
            return null;
        }

        function applyLineHeight(value) {
            restoreSelection();
            const block = closestBlock();
            (block || surface).style.lineHeight = value;
            emit();
        }

        function closeMenus() {
            ribbon.querySelectorAll('.word-menu').forEach(menu => { menu.hidden = true; });
        }

        function toggleMenu(name) {
            const menu = ribbon.querySelector('[data-menu="' + name + '"]')?.parentElement?.querySelector('.word-menu');
            if (!menu) return;
            const open = menu.hidden;
            closeMenus();
            menu.hidden = !open;
        }

        async function pasteFromClipboard() {
            saveSelection();
            let html = '';
            let text = '';
            try {
                if (navigator.clipboard.read) {
                    const items = await navigator.clipboard.read();
                    for (const item of items) {
                        if (!html && item.types.includes('text/html'))
                            html = await (await item.getType('text/html')).text();
                        else if (!text && item.types.includes('text/plain'))
                            text = await (await item.getType('text/plain')).text();
                    }
                }
            } catch { /* permission or unsupported clipboard read */ }
            if (!html && !text) {
                try { text = await navigator.clipboard.readText(); } catch { /* ignore */ }
            }
            restoreSelection();
            if (html) document.execCommand('insertHTML', false, sanitizeHtml(html));
            else if (text) document.execCommand('insertText', false, stripOfficeJunk(text));
            else document.execCommand('paste');
            emit();
        }

        function capturePainter() {
            if (paintStyle) {
                paintStyle = null;
                surface.classList.remove('is-painting');
                ribbon.querySelector('[data-cmd="painter"]')?.classList.remove('is-active');
                return;
            }
            restoreSelection();
            const sel = window.getSelection();
            let node = sel && sel.anchorNode;
            if (node && node.nodeType === Node.TEXT_NODE) node = node.parentElement;
            if (!inSurface(node)) node = surface;
            const cs = getComputedStyle(node);
            const parts = [
                'font-weight: ' + cs.fontWeight,
                'font-style: ' + cs.fontStyle,
                'text-decoration: ' + cs.textDecorationLine,
                'font-size: ' + cs.fontSize,
                'font-family: ' + cs.fontFamily,
                'color: ' + cs.color
            ];
            if (cs.backgroundColor && cs.backgroundColor !== 'rgba(0, 0, 0, 0)')
                parts.push('background-color: ' + cs.backgroundColor);
            paintStyle = parts.join('; ');
            surface.classList.add('is-painting');
            ribbon.querySelector('[data-cmd="painter"]')?.classList.add('is-active');
        }

        function syncState() {
            const sel = window.getSelection();
            if (!sel || !sel.rangeCount || !inSurface(sel.anchorNode)) return;
            ribbon.querySelectorAll('[data-state]').forEach(btn => {
                let on = false;
                try { on = document.queryCommandState(btn.dataset.state); } catch { on = false; }
                btn.classList.toggle('is-active', !!on);
            });
            let node = sel.anchorNode;
            if (node && node.nodeType === Node.TEXT_NODE) node = node.parentElement;
            if (!inSurface(node)) return;
            const cs = getComputedStyle(node);
            const family = (cs.fontFamily || '').split(',')[0].replace(/['"]/g, '').trim();
            if ([...fontSelect.options].some(opt => opt.value.toLowerCase() === family.toLowerCase()))
                fontSelect.value = [...fontSelect.options].find(opt => opt.value.toLowerCase() === family.toLowerCase()).value;
            const pt = Math.round(parseFloat(cs.fontSize) * 0.75);
            if (SIZES.includes(pt)) sizeSelect.value = String(pt);
        }

        function activeCell() {
            const sel = window.getSelection();
            let node = sel && sel.anchorNode;
            if (!node) return null;
            if (node.nodeType === Node.TEXT_NODE) node = node.parentElement;
            if (!node || !node.closest) return null;
            const cell = node.closest('td, th');
            return cell && surface.contains(cell) ? cell : null;
        }

        function activeTable() {
            const cell = activeCell();
            if (cell) return { cell, table: cell.closest('table'), row: cell.parentElement };
            const tables = surface.querySelectorAll('table');
            const table = tables[tables.length - 1];
            if (!table || !table.rows.length) return null;
            const row = table.rows[table.rows.length - 1];
            return { cell: row.cells[0] || null, table, row };
        }

        function placeCaret(cell) {
            if (!cell) return;
            const range = document.createRange();
            range.selectNodeContents(cell);
            range.collapse(true);
            const sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(range);
            savedRange = range.cloneRange();
        }

        function insertTable(rows, cols) {
            rows = Math.max(1, Math.min(20, rows | 0));
            cols = Math.max(1, Math.min(12, cols | 0));
            const each = 110;
            const table = document.createElement('table');
            table.style.width = (each * cols) + 'px';
            table.style.tableLayout = 'fixed';
            const body = document.createElement('tbody');
            for (let r = 0; r < rows; r++) {
                const tr = document.createElement('tr');
                for (let c = 0; c < cols; c++) {
                    const td = document.createElement('td');
                    td.style.width = each + 'px';
                    td.style.height = '36px';
                    td.appendChild(document.createElement('br'));
                    tr.appendChild(td);
                }
                body.appendChild(tr);
            }
            table.appendChild(body);
            restoreSelection();
            const sel = window.getSelection();
            if (sel && sel.rangeCount && inSurface(sel.getRangeAt(0).commonAncestorContainer)) {
                const range = sel.getRangeAt(0);
                range.collapse(false);
                range.insertNode(table);
            } else {
                surface.appendChild(table);
            }
            placeCaret(table.querySelector('td'));
            emit();
        }

        function visualRow(cell) {
            const table = cell.closest('table');
            if (!table) return cell.parentElement ? cell.parentElement.rowIndex : -1;
            const pos = cellOrigin(tableGrid(table), cell);
            if (!pos) return cell.parentElement ? cell.parentElement.rowIndex : -1;
            const rows = [];
            for (let r = pos.r; r < pos.r + pos.rs; r += 1) rows.push(r);
            return rows;
        }

        function deleteVisualRow(table, index) {
            const grid = tableGrid(table);
            const seen = new Set();
            (grid[index] || []).forEach(cell => {
                if (!cell || seen.has(cell)) return;
                seen.add(cell);
                const pos = cellOrigin(grid, cell);
                if (!pos) return;
                if ((cell.rowSpan || 1) > 1) cell.rowSpan = (cell.rowSpan || 1) - 1;
                else cell.remove();
            });
            [...table.rows].forEach(row => { if (!row.cells.length) row.remove(); });
            if (!table.querySelector('td, th')) table.remove();
        }

        function addRow(remove) {
            if (remove) {
                const selected = chosenCells();
                const focus = selected[0] || activeCell() || cellFromSavedRange();
                if (!focus) return;
                const table = focus.closest('table');
                if (!table) return;
                const indexes = (selected.length ? selected : [focus]).flatMap(visualRow);
                [...new Set(indexes)].filter(index => index >= 0).sort((a, b) => b - a).forEach(index => {
                    if (table.isConnected) deleteVisualRow(table, index);
                });
                clearCellSelection();
                emit();
                return;
            }
            const found = activeTable();
            if (!found) return;
            const tr = document.createElement('tr');
            const count = found.row.cells.length || 1;
            for (let i = 0; i < count; i++) {
                const td = document.createElement('td');
                const sample = found.row.cells[i];
                if (sample && sample.style.width) td.style.width = sample.style.width;
                td.style.height = (sample && sample.style.height) || '36px';
                td.appendChild(document.createElement('br'));
                tr.appendChild(td);
            }
            found.row.after(tr);
            placeCaret(tr.cells[0]);
            emit();
        }

        function cellFromSavedRange() {
            const node = savedRange && savedRange.startContainer;
            const el = node && (node.nodeType === 1 ? node : node.parentElement);
            const cell = el && el.closest && el.closest('td, th');
            return cell && surface.contains(cell) ? cell : null;
        }

        function visualColumn(cell) {
            const table = cell.closest('table');
            if (!table) return cell.cellIndex;
            const pos = cellOrigin(tableGrid(table), cell);
            return pos ? pos.c : cell.cellIndex;
        }

        function deleteVisualColumn(table, index) {
            const grid = tableGrid(table);
            const seen = new Set();
            for (let r = 0; r < grid.length; r += 1) {
                const cell = grid[r] && grid[r][index];
                if (!cell || seen.has(cell)) continue;
                seen.add(cell);
                const pos = cellOrigin(grid, cell);
                if (!pos) continue;
                if ((cell.colSpan || 1) > 1) cell.colSpan = (cell.colSpan || 1) - 1;
                else cell.remove();
            }
            const group = table.querySelector('colgroup');
            if (group && group.children[index]) group.children[index].remove();
            [...table.rows].forEach(row => { if (!row.cells.length) row.remove(); });
            if (!table.querySelector('td, th')) table.remove();
        }

        function addColumn(remove) {
            if (remove) {
                const selected = chosenCells();
                const focus = selected[0] || activeCell() || cellFromSavedRange();
                if (!focus) return;
                const table = focus.closest('table');
                if (!table) return;
                const indexes = (selected.length ? selected : [focus])
                    .map(visualColumn)
                    .filter(index => index >= 0);
                [...new Set(indexes)].sort((a, b) => b - a).forEach(index => {
                    if (table.isConnected) deleteVisualColumn(table, index);
                });
                clearCellSelection();
                emit();
                return;
            }
            const cell = activeCell() || cellFromSavedRange();
            if (!cell) return;
            const table = cell.closest('table');
            const grid = tableGrid(table);
            const pos = cellOrigin(grid, cell);
            if (!pos) return;
            const insertAt = pos.c + pos.cs;
            const spanned = new Set();
            for (let r = 0; r < grid.length; r += 1) {
                const covered = grid[r] && grid[r][insertAt - 1];
                const origin = covered && cellOrigin(grid, covered);
                const row = table.rows[r];
                if (!row) continue;
                if (origin && origin.cs > 1 && origin.c < insertAt && origin.c + origin.cs > insertAt) {
                    if (!spanned.has(covered)) {
                        spanned.add(covered);
                        covered.colSpan = (covered.colSpan || 1) + 1;
                    }
                    continue;
                }
                const tag = (covered && covered.tagName === 'TH') || (row.parentElement && row.parentElement.tagName === 'THEAD') ? 'th' : 'td';
                const td = document.createElement(tag);
                td.style.width = '110px';
                td.style.height = (covered && covered.style.height) || '36px';
                td.appendChild(document.createElement('br'));
                if (covered && covered.parentElement === row) covered.after(td);
                else row.appendChild(td);
            }
            const group = table.querySelector('colgroup');
            if (group) {
                const col = document.createElement('col');
                col.style.width = '110px';
                group.insertBefore(col, group.children[insertAt] || null);
            }
            const current = parseInt((table.style.width || '').replace('px', ''), 10);
            if (current) table.style.width = (current + 110) + 'px';
            const placed = tableGrid(table)[pos.r] && tableGrid(table)[pos.r][insertAt];
            placeCaret(placed || cell);
            emit();
        }

        function scaleTable(factor) {
            const found = activeTable();
            if (!found) return;
            const table = found.table;
            const max = Math.max(160, surface.clientWidth - 24);
            const current = table.getBoundingClientRect().width;
            const next = Math.max(160, Math.min(max, Math.round(current * factor)));
            table.style.width = next + 'px';
            table.style.tableLayout = 'fixed';
            emit();
        }

        let columnDrag = null;

        function onColumnDrag(event) {
            if (!columnDrag) return;
            const next = Math.max(48, columnDrag.startWidth + (event.clientX - columnDrag.startX));
            columnDrag.cells.forEach(cell => { cell.style.width = next + 'px'; });
            columnDrag.table.style.width = Math.max(120, columnDrag.tableWidth + (next - columnDrag.startWidth)) + 'px';
            columnDrag.table.style.tableLayout = 'fixed';
        }

        function stopColumnDrag() {
            if (!columnDrag) return;
            columnDrag = null;
            document.removeEventListener('mousemove', onColumnDrag);
            document.removeEventListener('mouseup', stopColumnDrag);
            emit();
        }

        surface.addEventListener('mousemove', event => {
            if (columnDrag) return;
            const cell = event.target.closest && event.target.closest('td, th');
            if (!cell || !surface.contains(cell)) {
                surface.classList.remove('is-col-resize');
                return;
            }
            const rect = cell.getBoundingClientRect();
            surface.classList.toggle('is-col-resize', rect.right - event.clientX <= 7 && rect.right - event.clientX >= -2);
        });

        function tableGrid(table) {
            const grid = [];
            [...table.rows].forEach((row, r) => {
                grid[r] = grid[r] || [];
                let c = 0;
                [...row.cells].forEach(cell => {
                    while (grid[r][c]) c += 1;
                    const rs = cell.rowSpan || 1;
                    const cs = cell.colSpan || 1;
                    for (let rr = 0; rr < rs; rr += 1) {
                        grid[r + rr] = grid[r + rr] || [];
                        for (let cc = 0; cc < cs; cc += 1) grid[r + rr][c + cc] = cell;
                    }
                    c += cs;
                });
            });
            return grid;
        }

        function cellOrigin(grid, cell) {
            for (let r = 0; r < grid.length; r += 1) {
                const row = grid[r] || [];
                for (let c = 0; c < row.length; c += 1) {
                    if (row[c] === cell) return { r, c, rs: cell.rowSpan || 1, cs: cell.colSpan || 1 };
                }
            }
            return null;
        }

        function clearCellSelection() {
            surface.querySelectorAll('.is-cell-selected').forEach(cell => cell.classList.remove('is-cell-selected'));
        }

        function rectangle(anchor, current) {
            const table = anchor.closest('table');
            if (!table || !current || current.closest('table') !== table) return null;
            const grid = tableGrid(table);
            const a = cellOrigin(grid, anchor);
            const b = cellOrigin(grid, current);
            if (!a || !b) return null;
            let r1 = Math.min(a.r, b.r);
            let c1 = Math.min(a.c, b.c);
            let r2 = Math.max(a.r + a.rs - 1, b.r + b.rs - 1);
            let c2 = Math.max(a.c + a.cs - 1, b.c + b.cs - 1);
            const cells = new Set();
            let grew = true;
            while (grew) {
                grew = false;
                for (let r = r1; r <= r2; r += 1) {
                    for (let c = c1; c <= c2; c += 1) {
                        const cell = grid[r] && grid[r][c];
                        if (!cell) continue;
                        cells.add(cell);
                        const pos = cellOrigin(grid, cell);
                        const nr1 = Math.min(r1, pos.r);
                        const nc1 = Math.min(c1, pos.c);
                        const nr2 = Math.max(r2, pos.r + pos.rs - 1);
                        const nc2 = Math.max(c2, pos.c + pos.cs - 1);
                        if (nr1 !== r1 || nc1 !== c1 || nr2 !== r2 || nc2 !== c2) {
                            r1 = nr1; c1 = nc1; r2 = nr2; c2 = nc2;
                            grew = true;
                        }
                    }
                }
            }
            return { table, grid, cells: [...cells], r1, c1, r2, c2 };
        }

        function paintCells(anchor, current) {
            const area = rectangle(anchor, current);
            if (!area) return;
            clearCellSelection();
            surface.classList.add('is-cell-drag');
            area.cells.forEach(cell => cell.classList.add('is-cell-selected'));
            const sel = window.getSelection();
            if (sel) sel.removeAllRanges();
        }

        function lockColumnWidths(table) {
            const grid = tableGrid(table);
            const count = grid.reduce((max, row) => Math.max(max, row.length), 0);
            const widths = [];
            for (let c = 0; c < count; c += 1) {
                let px = 0;
                for (let r = 0; r < grid.length && !px; r += 1) {
                    const cell = grid[r][c];
                    const pos = cell && cellOrigin(grid, cell);
                    if (pos && pos.c === c && pos.cs === 1) px = Math.round(cell.getBoundingClientRect().width);
                }
                widths.push(px || 110);
            }
            let group = table.querySelector('colgroup');
            if (!group) {
                group = document.createElement('colgroup');
                table.insertBefore(group, table.firstChild);
            }
            group.replaceChildren();
            widths.forEach(px => {
                const col = document.createElement('col');
                col.style.width = px + 'px';
                group.appendChild(col);
            });
            table.style.tableLayout = 'fixed';
            table.style.width = widths.reduce((sum, px) => sum + px, 0) + 'px';
            table.querySelectorAll('td, th').forEach(cell => { cell.style.width = ''; });
        }

        function mergeCells() {
            const selected = [...surface.querySelectorAll('td.is-cell-selected, th.is-cell-selected')];
            if (selected.length < 2) return;
            const table = selected[0].closest('table');
            const grid = tableGrid(table);
            let r1 = Infinity;
            let c1 = Infinity;
            let r2 = -1;
            let c2 = -1;
            selected.forEach(cell => {
                const pos = cellOrigin(grid, cell);
                if (!pos) return;
                r1 = Math.min(r1, pos.r);
                c1 = Math.min(c1, pos.c);
                r2 = Math.max(r2, pos.r + pos.rs - 1);
                c2 = Math.max(c2, pos.c + pos.cs - 1);
            });
            for (let r = r1; r <= r2; r += 1) {
                for (let c = c1; c <= c2; c += 1) {
                    if (!grid[r] || !selected.includes(grid[r][c])) return;
                }
            }
            const anchor = grid[r1][c1];
            const extra = selected.filter(cell => cell !== anchor && (cell.innerText || '').replace(/\u200b/g, '').trim());
            if (extra.length) {
                const html = extra.map(cell => cell.innerHTML).join('<br>');
                const own = (anchor.innerText || '').replace(/\u200b/g, '').trim();
                anchor.insertAdjacentHTML('beforeend', (own ? '<br>' : '') + html);
            }
            lockColumnWidths(table);
            anchor.colSpan = c2 - c1 + 1;
            anchor.rowSpan = r2 - r1 + 1;
            selected.forEach(cell => { if (cell !== anchor) cell.remove(); });
            clearCellSelection();
            placeCaret(anchor);
            emit();
        }

        let cellSelect = null;

        surface.addEventListener('mousedown', event => {
            if (event.button !== undefined && event.button !== 0) return;
            const cell = event.target.closest && event.target.closest('td, th');
            if (!cell || !surface.contains(cell)) {
                clearCellSelection();
                return;
            }
            const rect = cell.getBoundingClientRect();
            if (rect.right - event.clientX <= 7) {
                event.preventDefault();
                const table = cell.closest('table');
                columnDrag = {
                    table,
                    cells: [...table.rows].map(row => row.cells[cell.cellIndex]).filter(Boolean),
                    startX: event.clientX,
                    startWidth: rect.width,
                    tableWidth: table.getBoundingClientRect().width
                };
                table.style.tableLayout = 'fixed';
                document.addEventListener('mousemove', onColumnDrag);
                document.addEventListener('mouseup', stopColumnDrag);
                return;
            }
            cellSelect = { anchor: cell, x: event.clientX, y: event.clientY, moved: false };
        });

        document.addEventListener('mousemove', event => {
            if (!cellSelect || columnDrag) return;
            const hit = document.elementFromPoint(event.clientX, event.clientY);
            const cell = hit && hit.closest && hit.closest('td, th');
            if (!cell || !surface.contains(cell) || cell === cellSelect.anchor) return;
            cellSelect.moved = true;
            surface.classList.add('is-cell-drag');
            paintCells(cellSelect.anchor, cell);
        });

        document.addEventListener('mouseup', () => {
            if (!cellSelect) return;
            const moved = cellSelect.moved;
            cellSelect = null;
            surface.classList.remove('is-cell-drag');
            if (!moved) clearCellSelection();
        });

        ribbon.addEventListener('mousedown', event => {
            const sel = window.getSelection();
            if (sel && sel.rangeCount) {
                const range = sel.getRangeAt(0);
                if (inSurface(range.commonAncestorContainer) && (!range.collapsed || !savedRange || savedRange.collapsed))
                    savedRange = range.cloneRange();
            }
            if (event.target.closest('select, input, .word-image-btn')) return;
            event.preventDefault();
        });

        function applyPalette(kind, value) {
            const bar = kind === 'font' ? fontBar : highlightBar;
            if (kind === 'font') {
                const color = value === 'none' ? '#222222' : value;
                bar.style.background = color;
                paintSelection('font', color);
            } else if (value === 'none') {
                bar.style.background = '#fff59d';
                paintSelection('highlight', 'transparent');
            } else {
                bar.style.background = value;
                paintSelection('highlight', value);
            }
            closeMenus();
        }

        ribbon.addEventListener('click', event => {
            const swatch = event.target.closest('[data-swatch]');
            if (swatch) {
                applyPalette(swatch.dataset.kind, swatch.dataset.swatch);
                return;
            }
            if (event.target.closest('[data-highlight-stop]')) {
                closeMenus();
                return;
            }
            const more = event.target.closest('[data-more]');
            if (more) {
                ribbon.querySelector('.word-more-input[data-kind="' + more.dataset.more + '"]')?.click();
                return;
            }
            const menuBtn = event.target.closest('[data-menu]');
            if (menuBtn) {
                toggleMenu(menuBtn.dataset.menu);
                return;
            }
            const caseBtn = event.target.closest('[data-case]');
            if (caseBtn) {
                closeMenus();
                changeCase(caseBtn.dataset.case);
                return;
            }
            const spaceBtn = event.target.closest('[data-space]');
            if (spaceBtn) {
                closeMenus();
                applyLineHeight(spaceBtn.dataset.space);
                return;
            }
            const btn = event.target.closest('[data-cmd]');
            if (!btn || btn.tagName === 'SELECT' || btn.tagName === 'INPUT') return;
            closeMenus();
            const cmd = btn.dataset.cmd;
            if (cmd === 'paste') { pasteFromClipboard(); return; }
            if (cmd === 'cut' || cmd === 'copy') { exec(cmd); return; }
            if (cmd === 'painter') { capturePainter(); return; }
            if (cmd === 'grow') { bumpSize(1); return; }
            if (cmd === 'shrink') { bumpSize(-1); return; }
            if (cmd === 'bold') { toggleWrap('B'); return; }
            if (cmd === 'italic') { toggleWrap('I'); return; }
            if (cmd === 'underline') { toggleWrap('U'); return; }
            if (cmd === 'strikeThrough') { toggleWrap('S'); return; }
            if (cmd === 'clear') { exec('removeFormat'); return; }
            if (cmd === 'insertTable') {
                const rows = Number(ribbon.querySelector('.word-table-rows')?.value) || 3;
                const cols = Number(ribbon.querySelector('.word-table-cols')?.value) || 2;
                insertTable(rows, cols);
                return;
            }
            if (cmd === 'addRow') { addRow(false); return; }
            if (cmd === 'delRow') { addRow(true); return; }
            if (cmd === 'addCol') { addColumn(false); return; }
            if (cmd === 'delCol') { addColumn(true); return; }
            if (cmd === 'mergeCells') { mergeCells(); return; }
            if (cmd === 'tableSmaller') {
                if (!scaleImage(0.85)) scaleTable(0.85);
                return;
            }
            if (cmd === 'tableBigger') {
                if (!scaleImage(1.15)) scaleTable(1.15);
                return;
            }
            exec(cmd);
        });

        function insertImageFile(file) {
            if (!file || !/^image\/(png|jpeg|gif|webp)$/i.test(file.type)) return;
            if (file.size > 1572864) {
                window.alert('Image must be smaller than 1.5 MB.');
                return;
            }
            const reader = new FileReader();
            reader.onload = () => {
                const src = String(reader.result || '');
                if (!isSafeImageSrc(src)) return;
                const img = document.createElement('img');
                img.src = src;
                img.alt = '';
                img.style.width = '360px';
                img.style.height = 'auto';
                restoreSelection();
                const sel = window.getSelection();
                if (sel && sel.rangeCount && inSurface(sel.getRangeAt(0).commonAncestorContainer)) {
                    const range = sel.getRangeAt(0);
                    range.collapse(false);
                    range.insertNode(img);
                } else {
                    surface.appendChild(img);
                }
                emit();
            };
            reader.readAsDataURL(file);
        }

        ribbon.querySelector('.word-image-input')?.addEventListener('change', event => {
            const file = event.target.files && event.target.files[0];
            event.target.value = '';
            if (file) insertImageFile(file);
        });

        fontSelect.addEventListener('change', () => {
            const cells = chosenCells();
            if (!cells.length) { exec('fontName', fontSelect.value); return; }
            checkpoint();
            cells.forEach(cell => { cell.style.fontFamily = fontSelect.value; });
            emit();
        });
        sizeSelect.addEventListener('change', () => applyFontSize(Number(sizeSelect.value)));

        ribbon.querySelectorAll('.word-more-input').forEach(input => {
            input.addEventListener('change', () => applyPalette(input.dataset.kind, input.value));
        });

        surface.addEventListener('mouseup', () => {
            saveSelection();
            if (!paintStyle) return;
            const sel = window.getSelection();
            if (!sel || !sel.rangeCount || sel.isCollapsed) return;
            const range = sel.getRangeAt(0);
            if (!inSurface(range.commonAncestorContainer)) return;
            const span = document.createElement('span');
            span.setAttribute('style', filterStyle(paintStyle));
            span.appendChild(range.extractContents());
            range.insertNode(span);
            paintStyle = null;
            surface.classList.remove('is-painting');
            ribbon.querySelector('[data-cmd="painter"]')?.classList.remove('is-active');
            emit();
        });

        surface.addEventListener('keyup', saveSelection);
        surface.addEventListener('input', emit);
        surface.addEventListener('blur', emit);
        surface.addEventListener('paste', event => {
            const items = event.clipboardData?.items;
            if (items) {
                for (const item of items) {
                    if (item.type && item.type.indexOf('image/') === 0) {
                        const file = item.getAsFile();
                        if (file) {
                            event.preventDefault();
                            insertImageFile(file);
                            return;
                        }
                    }
                }
            }
            const html = event.clipboardData?.getData('text/html');
            const text = event.clipboardData?.getData('text/plain');
            if (!html && text == null) return;
            event.preventDefault();
            if (html) document.execCommand('insertHTML', false, sanitizeHtml(html));
            else document.execCommand('insertText', false, stripOfficeJunk(text || ''));
            emit();
        });

        textarea.closest('form')?.addEventListener('submit', () => emit());
        document.addEventListener('selectionchange', syncState);
        document.addEventListener('click', event => {
            if (!ribbon.contains(event.target)) closeMenus();
        });

        loadInitial(textarea.value || '');
        emit();
        checkpoint();
        document.addEventListener('keydown', event => {
            if (!wrap.contains(event.target)) return;
            if (event.target.closest('input, textarea')) return;
            const meta = event.ctrlKey || event.metaKey;
            if (!meta || event.altKey) return;
            const key = event.key.toLowerCase();
            if (key !== 'z' && key !== 'y') return;
            event.preventDefault();
            if (key === 'y' || event.shiftKey) redoEdit();
            else undoEdit();
        });
    }

    window.WordDescriptionEditor = { attach, sanitizeHtml };
})();
